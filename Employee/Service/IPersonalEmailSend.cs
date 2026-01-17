using EMPLOYEE.Setting;

namespace EMPLOYEE.Service
{
    public interface IPersonalEmailSend
    {
        Task<string> AddAsync(EmailSettings modelstring,string email );
        Task<string> SendWithMyEmailAsync(string email,string to, string subject, string htmlBody);
    }
}
