using EMPLOYEE.Application.Features.Employees.EmployeeDtos;
using EMPLOYEE.Models;

namespace EMPLOYEE.Repository
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<List<EmployeeWithManagerDto>> GetAllEmployeesAsync();
        Task<List<EmployeeWithManagerDto>> GetByManagerAsync(string? managerId);
        Task<EmployeeDto?> GetByEmailAsync(string? email);
        Task<Department?> GetDepartmentForManagerAsync(string managerUserId);
        Task<bool> CanManageEmployeeAsync(int employeeId, string callerId, string callerRole);
        Task<bool> DepartmentExistsAsync(int departmentId);
        Task<bool> HasCurrentMonthEvaluationAsync(int employeeId);


    }

}
