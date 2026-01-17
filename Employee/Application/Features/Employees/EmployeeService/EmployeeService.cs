using EMPLOYEE.Application.Common;
using EMPLOYEE.Application.Features.Employees.EmployeeDtos;
using EMPLOYEE.Application.Features.Employees.EmployeeService;
using EMPLOYEE.Data;
using EMPLOYEE.Models;
using EMPLOYEE.Repository;
using EMPLOYEE.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class EmployeeService : IEmployeeService
{
   
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmployeeRepository _repo;

    public EmployeeService( UserManager<ApplicationUser> userManager, IEmployeeRepository repo)
    {
        _userManager = userManager;
        _repo = repo;
    }


    public async Task<Result<List<EmployeeWithManagerDto>>> GetEmployeesByManagerAsync(string? managerUserId)
    {
        var employees = await _repo.GetByManagerAsync(managerUserId);
        return Result<List<EmployeeWithManagerDto>>.Ok(employees);
    }

    // ================= Add Employee =================
    public async Task<String> AddAsync(AddEmployeeDto model, string? creatorId)
    {
        if (await _userManager.FindByEmailAsync(model.Email) != null)
            return "Email already exists";

        var user = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return "Failed Add user";

        await _userManager.AddToRoleAsync(user, "Employee");
        var userId = await _userManager.FindByEmailAsync(model.Email);

        var dep = await _repo.GetDepartmentForManagerAsync(creatorId!);
        if (dep == null)
            return "Manager does not have an assigned department";
       

        var emp = new Employee
        {
            Position=model.Position,
            Salary=model.Salary,
            DepartmentId=dep.Id,
            UserId=userId.Id
        };

        _repo.AddAsync(emp);
        await _repo.SaveChangesAsync();
        return "Employee has been successfully added.";
    }

    // ================= Update Employee =================
    public async Task<string> UpdateAsync(UpdateEmployeeDto model, string? role, string? userId)
    {
        var employee = await _repo.GetByIdAsync(model.EmployeeId);
        if (employee == null)
            return "Employee not found";

        bool canManage = role == "Admin" || await _repo.CanManageEmployeeAsync(model.EmployeeId, userId!, role);
        if (!canManage) return "You are not authorized to update this employee.";
        if (model.Salary < 0)
            return "Salary cannot be negative";

        if (string.IsNullOrWhiteSpace(model.Position))
            return "Position is required";

        if (model.Salary.HasValue)
        {
            employee.Salary = model.Salary.Value;
        }

        if (!string.IsNullOrWhiteSpace(model.Position))
            employee.Position = model.Position;

        _repo.Update(employee);
        await _repo.SaveChangesAsync();

        return "Employee has been successfully updated.";
    }

    // ================= Delete Employee =================
    public async Task<string> DeleteAsync(int employeeId, string callerId, string callerRole)
    {
        var emp = await _repo.GetByIdAsync(employeeId);
        if (emp == null)
            return "Employee not found.";

        if (!emp.IsActive)
            return "Employee is already inactive.";

        bool canManage = callerRole == "Admin"
                || await _repo.CanManageEmployeeAsync(employeeId, callerId, callerRole);

        if (!canManage)
            return "You are not authorized to delete this employee.";

        emp.IsActive = false;
        _repo.Update(emp);
        await _repo.SaveChangesAsync();

        return "Employee has been successfully deactivated.";
    }

    // ================= Activate Employee =================
    public async Task<string> ActiveAsync(int employeeId, string callerId, string callerRole)
    {
        var emp = await _repo.GetByIdAsync(employeeId);
        if (emp == null)
            return "Employee not found.";

        if (emp.IsActive)
            return "Employee is already active.";

        bool canManage = callerRole == "Admin"
             || await _repo.CanManageEmployeeAsync(employeeId, callerId, callerRole);

        if (!canManage)
            return "You are not authorized to activate this employee.";

        emp.IsActive = true;
        _repo.Update(emp);
        await _repo.SaveChangesAsync();

        return "Employee has been successfully activated.";
    }

    public async Task<string> TransferEmployeeAsync(
    TransferEmployeeDepartmentDto dto,
    string adminUserId)
    {
        if (string.IsNullOrEmpty(adminUserId))
            return "Unauthorized request";

        var employee = await _repo.GetByIdAsync(dto.EmployeeId);
        if (employee == null)
            return "Employee not found";

        if (employee.DepartmentId == dto.NewDepartmentId)
            return "Employee is already in this department";

        bool departmentExists = await _repo.DepartmentExistsAsync(dto.NewDepartmentId);
        if (!departmentExists)
            return "Target department not found";
        // تحقق من وجود تقييم في الشهر الحالي
        bool hasEvaluation = await _repo.HasCurrentMonthEvaluationAsync(dto.EmployeeId);
        if (hasEvaluation)
            return "Cannot transfer employee with evaluations in the current month.";

        employee.DepartmentId = dto.NewDepartmentId;

        _repo.Update(employee);
        await _repo.SaveChangesAsync();

        return "Employee transferred successfully";
    }



}
