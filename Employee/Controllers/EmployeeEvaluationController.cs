using EMPLOYEE.Application.Common;
using EMPLOYEE.Application.Features.Evaluations.EvaluationDtos;
using EMPLOYEE.Application.Features.Evaluations.EvaluationService;
using EMPLOYEE.Application.Features.Evaluations.Query;
using EMPLOYEE.Application.Features.Evaluations.Update;
using EMPLOYEE.Data;
using EMPLOYEE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EMPLOYEE.Controllers
{
    [ApiController]
    [Route("api/evaluations")]
    public class EmployeeEvaluationController : ControllerBase
    {
        private readonly IEmployeeEvaluationService _service;
        private readonly IEvaluationAppService _app;
        private readonly ApplicationDbContext _db;


        public EmployeeEvaluationController(IEmployeeEvaluationService service, ApplicationDbContext db, IEvaluationAppService app)
        {
            _service = service;
            _db = db;
            _app = app;
        }
    
        private string? GetCallerId() => User.FindFirst("uid")?.Value;
        private bool IsAdmin() => User.IsInRole("Admin");
        private string GetCallerRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateEvaluationDto dto)
        {
            var callerId = GetCallerId();
            if (callerId == null)
                return Unauthorized();
            var result = await _app.CreateAsync(dto, callerId!, IsAdmin());

            return result.Success
     ? Ok(result.Data )
     : BadRequest(new { result.Error });
        }
        [Authorize]

        [HttpDelete]
        //yyyy-MM
        public async Task<IActionResult> Delete([FromForm] EmployeeEvaluationByPeriodDto dto)
        {
            var callerId = GetCallerId();
            if (callerId == null)
                return Unauthorized();

            var result = await _app.DeleteAsync(dto, callerId, IsAdmin());

            return result.Success
      ? Ok()
      : BadRequest(new { error = result.Error });
        }
        [Authorize]

        [HttpPut("update")]
        //yyyy-MM
        public async Task<IActionResult> UpdateEvaluation([FromForm] EmployeeEvaluationByPeriodDto dto,
            [FromForm] UpdateEvaluationDto updateDto)
        {
            var callerId = User.FindFirst("uid")?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (callerId == null)
                return Unauthorized("User ID not found in token.");

            var result = await _app.UpdateAsync(dto, updateDto, callerId, isAdmin);

            return result.Success
     ? Ok(result.Data)
     : BadRequest(new { result.Error });
        }

        [Authorize]
        [HttpGet("all_yearly")]
        //Admin => AllEmp  ,, Manager => in his teem
        // If send month Time_line monthly Then yealy
        public async Task<IActionResult> GetMyTeamEvaluationsAsync([FromQuery] int? month, int? year)
        {
            var callerId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(callerId))
                return Unauthorized("User ID not found in token.");
           
            var evaluations = await _service.GetTeamEvaluationsAsync(callerId, IsAdmin() , year,month);

            return evaluations == null ? NotFound() : Ok(evaluations);
        }
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetbyId(int id)
        {
            var callerId = GetCallerId();
            if (string.IsNullOrEmpty(callerId))
                return Unauthorized("User ID not found in token.");

            var dto = await _service.GetByIdAsync(id, callerId, IsAdmin());

            return dto == null ? NotFound() : Ok(dto);
        }


        [Authorize]
        [HttpGet("by_emp_Uid")]
        public async Task<IActionResult> GetEvaluationsByEmployee()
        {
            var callerId = GetCallerId();
            if (string.IsNullOrEmpty(callerId))
                return Unauthorized("User ID not found in token.");
            var Eva = await _service.GetMyEvaluationsAsync(callerId);
            return Ok(Eva);

        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("employee/{employeeId:int}/dashboard/monthly")]
        public async Task<IActionResult> GetEmployeeMonthlyDashboard(int employeeId, [FromQuery]  int? year)
        {
           
            if (employeeId <= 0)
                return BadRequest("Invalid employee id.");

            var callerUid = GetCallerId();
            var callerRole = GetCallerRole();

            if (string.IsNullOrEmpty(callerUid))
                return Unauthorized("User ID not found in token.");
          
                var result = await _service.GetEmployeeMonthlyDashboardAsync(employeeId, callerUid, callerRole, year );
              
                return Ok(result);
            
          
        }

        [Authorize]

        [HttpGet("employee/{employeeId:int}/dashboard/yearly")]
        public async Task<IActionResult> GetEmployeeYearlyDashboard(int employeeId,[FromQuery] int? year)
        {
            if (employeeId <= 0)
                return BadRequest("Invalid employee id.");
            var callerUid = GetCallerId();
            var callerRole = GetCallerRole();

            if (string.IsNullOrEmpty(callerUid))
                return Unauthorized("User ID not found in token.");
          
            var result = await _service.GetEmployeeYearlyDashboardAsync(
                employeeId,
                callerUid!,
                callerRole!,
                year

            );
            return result == null ? NotFound() : Ok(result);
        }


    }
}
