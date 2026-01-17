
using EMPLOYEE.Setting;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace EMPLOYEE.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettind _emailSettings;

        public EmailService(IOptions<EmailSettind> options)
        {
            _emailSettings = options.Value ?? throw new Exception("Email settings are missing"); ;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("NoReply", _emailSettings.Email));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = htmlBody };

            using SmtpClient smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, _emailSettings.EnableSSL);
            await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }
    
    }
}
