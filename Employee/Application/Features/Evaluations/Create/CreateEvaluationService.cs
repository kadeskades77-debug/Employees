using EMPLOYEE.Application.Common;
using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Application.Features.Evaluations.EvaluationDtos;
using EMPLOYEE.Data;
using EmployeeDomain.Entities;
using EmployeeDomain.Exceptions;
using EmployeeDomain.ValueObjects;
using Microsoft.EntityFrameworkCore;
namespace EMPLOYEE.Application.Features.Evaluations.Create
{
    public class CreateEvaluationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IPermissionService _permissionService;

        public CreateEvaluationService(ApplicationDbContext db, IPermissionService permissionService)
        {
            _db = db;
            _permissionService = permissionService;
        }

        public async Task<Result<string>> HandleAsync(
            CreateEvaluationDto dto,
            string callerId,
            bool isAdmin)
        {
            bool canManage = await _permissionService.HasPermissionAsync(callerId, "Permissions.Evaluations.Manage");
            if (!canManage)
                return Result<string>.Fail("You do not have permission to Add Evaluation.");

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

            if (!isAdmin && department?.ManagerUserId != callerId)
                return Result<string>.Fail("Not in your team");

            var period = EvaluationPeriod.CurrentMonthUtc();

            bool exists = await _db.EmployeeEvaluations.AnyAsync(e =>
                e.EmployeeId == dto.EmployeeId &&
                e.Period == period.Value
            );

            if (exists)
                return Result<string>.Fail("Already evaluated");

            try
            {
                string evaluatorId = isAdmin
                    ? department?.ManagerUserId ?? throw new Exception("Employee's manager not found")
                    : callerId;

                var evaluation = EmployeeEvaluation.Create(
                    dto.EmployeeId,
                    evaluatorId,
                    period.Value,
                    dto.Attendance,
                    dto.Quality,
                    dto.Productivity,
                    dto.Teamwork,
                    dto.Comments
                );

                _db.EmployeeEvaluations.Add(evaluation);
                await _db.SaveChangesAsync();

                return Result<string>.Ok("Evaluation created successfully");
            }
            catch (DomainException ex)
            {
                return Result<string>.Fail(ex.Message);
            }
        }
    }
}
