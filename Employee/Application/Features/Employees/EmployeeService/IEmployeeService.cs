using EMPLOYEE.Application.Common;
using EMPLOYEE.Application.Features.Employees.EmployeeDtos;
using EMPLOYEE.Models;
using EMPLOYEE.Service;

namespace EMPLOYEE.Application.Features.Employees.EmployeeService
{
    public interface IEmployeeService
         {
        
            Task<Result<List<EmployeeWithManagerDto>>> GetEmployeesByManagerAsync(string? managerId);
            Task<string> AddAsync(AddEmployeeDto model, string? creatorId);
            Task<string> UpdateAsync(UpdateEmployeeDto model, string? role, string? userId);
            Task<string> DeleteAsync(int employeeId, string callerId, string callerRole);
            Task<string> ActiveAsync(int employeeId, string callerId, string callerRole);
            Task<string> TransferEmployeeAsync(TransferEmployeeDepartmentDto dto, string adminUserId);
        
        }


    
}
