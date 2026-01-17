using EMPLOYEE.Application.Common;
using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Application.Features.Evaluations.EvaluationDtos;
using EMPLOYEE.Data;
using EmployeeDomain.Entities;
using EmployeeDomain.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace EMPLOYEE.Application.Features.Evaluations.Update
{
    public class UpdateEvaluationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IPermissionService _permissionService;

        public UpdateEvaluationService(ApplicationDbContext db, IPermissionService permissionService)
        {
            _db = db;
            _permissionService = permissionService;
        }

        public async Task<Result<string>> HandleAsync(
             EmployeeEvaluationByPeriodDto dto,
             UpdateEvaluationDto updateDto,
             string callerId,
             bool isAdmin)
        {
            bool canManage = await _permissionService.HasPermissionAsync(callerId, "Permissions.Evaluations.Manage");
            if (!canManage)
                return Result<string>.Fail("You do not have permission to Update Evaluation.");

            var employee = await _db.Employees
                .Where(e => e.Id == dto.EmployeeId)
                .Select(e => new { e.Id, e.DepartmentId })
                .FirstOrDefaultAsync();

            if (employee == null)
                return Result<string>.Fail("Employee not found");

            var department = await _db.Departments
                .Where(d => d.Id == employee.DepartmentId)
                .Select(d => new { d.ManagerUserId })
                .FirstOrDefaultAsync();

            var evaluation = await _db.EmployeeEvaluations
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == dto.EmployeeId &&
                    e.Period.Year == dto.Period.Year &&
                    e.Period.Month == dto.Period.Month
                );

            if (evaluation == null)
                return Result<string>.Fail("Evaluation not found");

            if (!isAdmin && department?.ManagerUserId != callerId)
                return Result<string>.Fail("You cannot edit this evaluation");

            try
            {
                evaluation.Update(
                    updateDto.Attendance,
                    updateDto.Quality,
                    updateDto.Productivity,
                    updateDto.Teamwork,
                    updateDto.Comments
                );

                await _db.SaveChangesAsync();

                return Result<string>.Ok("Evaluation updated successfully");
            }
            catch (DomainException ex)
            {
                return Result<string>.Fail(ex.Message);
            }
        }
    }
}
