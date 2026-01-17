using EMPLOYEE.Application.Features.Departments.DepartmentsDtos;

namespace EMPLOYEE.Application.Features.Departments.DepartmentService
{
    public interface IDepartmentService
    {
        Task<string> CreateAsync(CreateDepartmentDto dto, string callerId);
        Task<string> UpdateAsync(int departmentId, UpdateDepartmentDto dto, string callerId);
        Task<string> ChangeManagerAsync(ChangeDepartmentManagerDto dto, string callerId);
        Task<List<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto?> GetMyDepartmentAsync(string managerUserId);
    }

}
