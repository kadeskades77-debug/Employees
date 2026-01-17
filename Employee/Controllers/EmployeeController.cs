using EMPLOYEE.Application.Features.Employees.EmployeeDtos;
using EMPLOYEE.Application.Features.Employees.EmployeeService;
using EMPLOYEE.Controllers.EMPLOYEE.Controllers;
using EMPLOYEE.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/employees")]
public class EmployeeController : BaseApiController
{
    private readonly IEmployeeService _service;
    private readonly IEmployeeRepository _repo;

    public EmployeeController(
        IEmployeeService service,
        IEmployeeRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    // ====================== Admin ======================

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _repo.GetAllEmployeesAsync();
        return Ok(employees);
    }

    // ====================== Manager ======================

    [Authorize(Roles = "Manager")]
    [HttpGet("my-employees")]
    public async Task<IActionResult> GetMyEmployees()
    {
        var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return FromResult(await _service.GetEmployeesByManagerAsync(managerId));
    }

    // ====================== Employee ======================

    [Authorize(Roles = "Employee")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return BadRequest("User email not found");

        var emp = await _repo.GetByEmailAsync(email);
        return emp == null ? NotFound() : Ok(emp);
    }

    // ====================== Add ======================

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("add")]
    public async Task<IActionResult> AddEmployee(AddEmployeeDto model)
    {
        var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var message = await _service.AddAsync(model, callerId);
        return Ok(new { message });
    }

    // ====================== Update ======================

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateEmployee(UpdateEmployeeDto model)
    {
        var callerRole = User.FindFirstValue(ClaimTypes.Role);
        var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (callerRole == null || callerId == null)
            return Unauthorized();

        var message = await _service.UpdateAsync(model, callerRole, callerId);
        return Ok(new { message });
        
    }

    // ====================== Delete ======================

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var callerRole = User.FindFirstValue(ClaimTypes.Role);
        var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (callerRole == null || callerId == null)
            return Unauthorized();

        var message = await _service.DeleteAsync(id, callerId, callerRole);
        return Ok(new { message });
    }

    // ====================== Activate ======================

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("active/{id}")]
    public async Task<IActionResult> ActiveEmployee(int id)
    {
        var callerRole = User.FindFirstValue(ClaimTypes.Role);
        var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (callerRole == null || callerId == null)
            return Unauthorized();

        var message = await _service.ActiveAsync(id, callerId, callerRole);
        return Ok(new { message });
    }

    // ====================== TransferEmployeeToOtherDepartment ======================

    [Authorize(Roles = "Admin")]
    [HttpPost("transfer")]
    public async Task<IActionResult> TransferEmployee(
    TransferEmployeeDepartmentDto dto)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var message = await _service.TransferEmployeeAsync(dto, adminId!);
        return Ok(new { message });
    }

}
