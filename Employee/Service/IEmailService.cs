namespace EMPLOYEE.Service
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);

    }
}
