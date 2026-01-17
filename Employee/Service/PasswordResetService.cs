using EMPLOYEE.Models;
using EMPLOYEE.Setting;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EMPLOYEE.Service
{
    public class PasswordResetService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSettind _emailSettings;

        public PasswordResetService(UserManager<ApplicationUser> userManager, IOptions<EmailSettind> emailSettings)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));
        }

        // توليد وإرسال كود إعادة تعيين كلمة المرور
        public async Task SendPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return; 

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string resetLink = $"http://localhost:7212/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

            string subject = "Password Reset Request";
            string body = $"<p>Click <a href='{resetLink}'>here</a> to reset your password.</p>";
            Console.WriteLine($"Reset link for {email}: {resetLink}");
             await SendEmailAsync(email, subject, body);
        }
        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("NoReply", _emailSettings.Email));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = htmlBody };

            using var smtp = new SmtpClient();
            var secureOption = _emailSettings.Port == 465
                   ? SecureSocketOptions.SslOnConnect
                   : SecureSocketOptions.StartTls;

            await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, secureOption);

            await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }
    }
}
