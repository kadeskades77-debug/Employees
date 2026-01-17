using EMPLOYEE.Application.Features.Departments.DepartmentsDtos;
using EMPLOYEE.Application.Features.Departments.DepartmentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMPLOYEE.Controllers
{
    [Route("api/Department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        // ================= Add =================
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddDepartment([FromBody] CreateDepartmentDto dto)
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.CreateAsync(dto, callerId!);
            return Ok(result);
        }

        // ================= Update =================
        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentDto dto)
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.UpdateAsync(id, dto, callerId!);
            return Ok(result);
        }
        // ================= Change Manager =================

        [Authorize]
        [HttpPut("change-manager")]
        public async Task<IActionResult> ChangeManager(
            [FromBody] ChangeDepartmentManagerDto dto)
        {

            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.ChangeManagerAsync(dto, callerId);

            return Ok(result);
        }
    

    // ================= Get All Departments =================

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // ================= My Department =================

        [Authorize(Roles = "Manager")]
        [HttpGet("my-department")]
        public async Task<IActionResult> GetMyDepartment()
        {
            var managerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.GetMyDepartmentAsync(managerUserId!);

            if (result == null)
                return NotFound("You are not assigned as a manager to any department.");

            return Ok(result);
        }


    }
}
