using EMPLOYEE.Application.Common;
using EMPLOYEE.Application.Dtos;
using EMPLOYEE.Models;

namespace EMPLOYEE.Application.Features.Auth
{
    public interface IAuthAppService
    {
        Task<Result<string>> LoginAsync(LoginDto dto);
        Task<Result> RegisterAsync(RegisterDto dto);
        Task<string> UpdateUserAsync(string callerId, UpdateUserDto dto, bool isAdmin);
        Task<Result> AddRoleAsync(AddRoleModel dto);
        Task<Result> RequestOtpAsync(RequestResetDto dto);
        Task<Result> VerifyOtpAsync(VerifyOtpDto dto);
        Task<Result> ResetPasswordOtpAsync(ResetPasswordOtpDto dto);
    }
}
