using EMPLOYEE.Data;
using EMPLOYEE.Models;
using EMPLOYEE.Service;
using Microsoft.AspNetCore.Identity;

namespace EMPLOYEE.Application.Abstractions
{
    public class OtpService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;

        public OtpService(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IEmailService emailService)
        {
            _userManager = userManager;
            _db = db;
            _emailService = emailService;
        }


        public async Task SendOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return;

            // Generate OTP (6 digits)
            var otp = new Random().Next(100000, 999999).ToString();

            // Save to DB
            var entry = new PasswordResetOtp
            {
                UserId = user.Id,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            _db.PasswordResetOtps.Add(entry);
            await _db.SaveChangesAsync();

            // Send OTP email
            await _emailService.SendAsync(email, "Your Reset Code", $"Your verification code is: <b>{otp}</b>");
        }
        // Validate OTP
        public async Task<bool> ValidateOtpAsync(string email, string otp)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var record = _db.PasswordResetOtps
                .Where(x => x.UserId == user.Id && x.OtpCode == otp)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if (record == null) return false;
            if (record.IsExpired) return false;

            return true;

        }
    }
    }
