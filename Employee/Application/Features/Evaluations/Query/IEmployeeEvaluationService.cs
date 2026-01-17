using EMPLOYEE.Application.Features.Evaluations.EvaluationDtos;
using EMPLOYEE.Models;

namespace EMPLOYEE.Application.Features.Evaluations.Query
{
    public interface IEmployeeEvaluationService
    {
        Task<EvaluationDto?> GetByIdAsync(int id, string? callerId, bool isAdmin);
        Task<EmployeeEvaluationsResponseDto?> GetMyEvaluationsAsync(string userId);

        //=====================Avrageand time line=====================================================
        Task<EmployeeYearlyDashboardDto?> GetEmployeeYearlyDashboardAsync(
    int employeeId,
    string? callerUid,
    string? callerRole,
    int? year = null
);

        Task<List<EmployeeEvaluationDashboardDto>> GetTeamEvaluationsAsync(
         string callerId,
         bool isAdmin,
         int? year = null,
         int? month = null
            );

        Task<EmployeeEvaluationDashboardDto?> GetEmployeeMonthlyDashboardAsync(
         int employeeId,
         string? callerUid,
         string? callerRole,
         int? year = null
     );
        //===================================================================================

    }

}
