using EMPLOYEE.Application.Abstractions;
using EMPLOYEE.Application.Dtos;
using EMPLOYEE.Application.Features.Auth;
using EMPLOYEE.Models;
using EMPLOYEE.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace EMPLOYEE.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly OtpService _otpService;
        private readonly IAuthAppService _auth;

        public AuthController(OtpService otpService,IAuthAppService auth)
        {
            _otpService = otpService;
            _auth = auth;
        }

        // ====================== REGISTER ======================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result = await _auth.RegisterAsync(model);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok("User registered successfully");
        }
        // ====================== Update ======================
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(UpdateUserDto dto)
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var message = await _auth.UpdateUserAsync(callerId, dto, isAdmin);
            return Ok(message);
        }

        // ====================== LOGIN ======================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _auth.LoginAsync(model);
            return result.Success ? Ok(new { token = result.Data }) : Unauthorized(result.Error);
        }

        // ====================== ADDROLE ======================
         [Authorize (Roles = "Admin")]
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRoleToUser([FromBody] AddRoleModel model)
        {
            var result = await _auth.AddRoleAsync(model);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok("Role added to user successfully");
        }

        // ====================== REQUEST OTP ======================
        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestResetDto model)
        {
            await _otpService.SendOtpAsync(model.Email);
            return Ok("OTP sent if user exists.");
        }

        // ====================== VERIFY OTP ======================
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto model)
        {
            var result = await _auth.VerifyOtpAsync(model);

            if (!result.Success)
                return BadRequest(result.Error);

            return Ok("OTP verified");
        }

        // ====================== RESET PASSWORD WITH OTP ======================
        [HttpPost("reset-password-otp")]
        public async Task<IActionResult> ResetPasswordOtp([FromBody] ResetPasswordOtpDto model)
        {
            var result = await _auth.ResetPasswordOtpAsync(model); 
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok("Password reset successfully");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminRoute()
        {
            return Ok("Admin allowed only");
        }



    }
}
