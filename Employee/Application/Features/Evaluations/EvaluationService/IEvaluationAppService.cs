using EMPLOYEE.Application.Common;
using EMPLOYEE.Application.Features.Evaluations.EvaluationDtos;
using EmployeeDomain.Entities;

namespace EMPLOYEE.Application.Features.Evaluations.EvaluationService
{
    public interface IEvaluationAppService
    {
        Task<Result<string>> CreateAsync(
            CreateEvaluationDto dto,
            string callerId,
            bool isAdmin);

        Task<Result<string>> UpdateAsync(
             EmployeeEvaluationByPeriodDto dto,
              UpdateEvaluationDto updateDto,
            string callerId,
            bool isAdmin);

        Task<Result<string>> DeleteAsync(
   EmployeeEvaluationByPeriodDto dto,
      string callerId,
      bool isAdmin);

    }

}
