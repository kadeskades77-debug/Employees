using EMPLOYEE.Data;
using EMPLOYEE.Migrations;
using EMPLOYEE.Setting;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MimeKit;


namespace EMPLOYEE.Service
{
    public class PersonalEmailSend : IPersonalEmailSend
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionService _encryptionService;
        public PersonalEmailSend(ApplicationDbContext context, IEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public async Task<string> AddAsync(EmailSettings model, string email)
        {

            if (string.IsNullOrWhiteSpace(model.Password))
                return "Email and Password is required.";
         model.Password=
      _encryptionService.Encrypt(model.Password);



            model.Email = email;

            _context.EmailSetting.Add(model);
            await _context.SaveChangesAsync();

            return "Email settings added successfully.";
        }

        public async Task<string> SendWithMyEmailAsync(string email, string to, string subject, string htmlBody)
        {
            var _emailSettings = await _context.EmailSetting.FirstOrDefaultAsync(x => x.Email == email);
            if (_emailSettings == null)
                return "Not found Email";
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_emailSettings.Email, _emailSettings.Email));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = htmlBody };

            using SmtpClient smtp = new SmtpClient();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            SecureSocketOptions socketOptions = _emailSettings.Port switch
            {
                465 => SecureSocketOptions.SslOnConnect,
                587 => SecureSocketOptions.StartTls,
                _ => SecureSocketOptions.StartTls
            };
            var password =
    _encryptionService.Decrypt(_emailSettings.Password);


            await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, socketOptions);

            await smtp.AuthenticateAsync(_emailSettings.Email, password);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
            return "Send Email is successfully";
        }
    }
}
