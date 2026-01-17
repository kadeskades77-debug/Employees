namespace EMPLOYEE.Application.Abstractions
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(string userId, string email, IList<string> roles);
    }
}
