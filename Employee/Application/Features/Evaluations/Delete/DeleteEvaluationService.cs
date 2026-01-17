using EMPLOYEE.Application.Common;
using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Application.Features.Evaluations.EvaluationDtos;
using EMPLOYEE.Data;
using EmployeeDomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMPLOYEE.Application.Features.Evaluations.Delete
{
    public class DeleteEvaluationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IPermissionService _permissionService;

        public DeleteEvaluationService(ApplicationDbContext db, IPermissionService permissionService)
        {
            _db = db;
            _permissionService = permissionService;
        }

        public async Task<Result<string>> HandleAsync(
            EmployeeEvaluationByPeriodDto dto,
            string callerId,
            bool isAdmin)
        {
            bool canManage = await _permissionService.HasPermissionAsync(callerId, "Permissions.Evaluations.Manage");
            if (!canManage)
                return Result<string>.Fail("You do not have permission to Delete Evaluation.");
            var evaluation = await _db.EmployeeEvaluations
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == dto.EmployeeId &&
                    e.Period.Year == dto.Period.Year &&
                    e.Period.Month == dto.Period.Month
                );

            if (evaluation == null)
                return Result<string>.Fail("Evaluation not found");

            var employee = await _db.Employees
                .Where(e => e.Id == evaluation.EmployeeId)
                .Select(e => new { e.DepartmentId })
                .FirstOrDefaultAsync();

            var department = await _db.Departments
                .Where(d => d.Id == employee.DepartmentId)
                .Select(d => new { d.ManagerUserId })
                .FirstOrDefaultAsync();

            if (!isAdmin && department?.ManagerUserId != callerId)
                return Result<string>.Fail("You cannot delete this evaluation");

            _db.EmployeeEvaluations.Remove(evaluation);
            await _db.SaveChangesAsync();

            return Result<string>.Ok("Evaluation deleted successfully");
        }
    }
}
