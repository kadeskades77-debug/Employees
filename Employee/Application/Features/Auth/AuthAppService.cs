using EMPLOYEE.Application.Abstractions;
using EMPLOYEE.Application.Common;
using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Application.Dtos;
using EMPLOYEE.Data;
using EMPLOYEE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Result = EMPLOYEE.Application.Common.Result;

namespace EMPLOYEE.Application.Features.Auth
{
    public class AuthAppService : IAuthAppService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly OtpService _otpService;

        public AuthAppService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtService jwtService,
            OtpService otpService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _otpService = otpService;
            _context = context;
        }

        public async Task<Result<string>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Result<string>.Fail("Wrong credentials");

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                return Result<string>.Fail("Wrong credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var token =await _jwtService.GenerateTokenAsync(user.Id, user.Email ?? "", roles);

            return Result<string>.Ok(token);
        }

        public async Task<Result> RegisterAsync(RegisterDto dto)
        {
            var exists = await _userManager.FindByEmailAsync(dto.Email);
            if (exists != null)
                return Result.Fail("Email already registered");

            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result.Fail(result.Errors.First().Description);

            await _userManager.AddToRoleAsync(user, "Employee");
            return Result.Ok();
        }
        public async Task<string> UpdateUserAsync(string callerId, UpdateUserDto dto, bool isAdmin)
        {
            var user = await _userManager.FindByIdAsync(callerId);
            if (user == null) return "User not found";

            bool nameChanged = false;
            if (!string.IsNullOrWhiteSpace(dto.FirstName) && dto.FirstName != user.FirstName)
            {
                user.FirstName = dto.FirstName;
                nameChanged = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.LastName) && dto.LastName != user.LastName)
            {
                user.LastName = dto.LastName;
                nameChanged = true;
            }

            // تحديث الايميل فقط للأدمين
            bool emailChanged = false;
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (!isAdmin)
                    return "Only Admin can update the email";

                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                    return "Email already in use";

                user.Email = dto.Email;
                user.UserName = dto.Email;
                emailChanged = true;
            }
            if (!nameChanged && !emailChanged)
                return "No changes to update";

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? "User updated successfully" : "Failed to update user";
        }

        public async Task<Result> AddRoleAsync(AddRoleModel dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Result.Fail("User not found");

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return Result.Fail("Role does not exist");

            if (await _userManager.IsInRoleAsync(user, dto.Role))
                return Result.Fail("User already has this role");

            await _userManager.AddToRoleAsync(user, dto.Role);
            return Result.Ok();
        }
        // ================= RequestOtp =================

        public async Task<Result> RequestOtpAsync(RequestResetDto dto)
        {
            await _otpService.SendOtpAsync(dto.Email);
            return Result.Ok();
        }

        public async Task<Result> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var valid = await _otpService.ValidateOtpAsync(dto.Email, dto.OtpCode);
            return valid ? Result.Ok() : Result.Fail("Invalid or expired OTP");
        }

        public async Task<Result> ResetPasswordOtpAsync(ResetPasswordOtpDto dto)
        {
            var valid = await _otpService.ValidateOtpAsync(dto.Email, dto.OtpCode);
            if (!valid)
                return Result.Fail("Invalid or expired OTP");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Result.Fail("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            return result.Succeeded
                ? Result.Ok()
                : Result.Fail(result.Errors.First().Description);
        }
    }
}
