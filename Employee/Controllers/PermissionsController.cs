using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/permissions")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _service;

    public PermissionsController(IPermissionService service)
    {
        _service = service;
    }

    // ================= Add Permission =================
    [Authorize(Roles = "Admin")]
    [HttpPost("add")]
    public async Task<IActionResult> AddPermission([FromBody] AddPermissionDto dto)
    {
        var result = await _service.AddPermissionAsync(dto);
        return Ok(result);
    }

    // ================= Assign Permission To Role =================
    [Authorize(Roles = "Admin")]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignPermissionToRole([FromBody] AssignPermissionDto dto)
    {
        var result = await _service.AssignPermissionToRoleAsync(dto);
        return Ok(result);
    }
}


