using EMPLOYEE.Application.Dtos;

namespace EMPLOYEE.Application.Common.Authorization
{
    public interface IPermissionService
    {
        Task<string> AddPermissionAsync(AddPermissionDto dto);
        Task<string> AssignPermissionToRoleAsync(AssignPermissionDto dto);
        Task<bool> HasPermissionAsync(string userId, string permissionName);
        Task<List<string>> GetPermissionsAsync(string userId);
    }

}
