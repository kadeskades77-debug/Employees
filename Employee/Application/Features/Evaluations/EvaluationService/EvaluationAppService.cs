using EMPLOYEE.Application.Common;
using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Application.Features.Evaluations.Create;
using EMPLOYEE.Application.Features.Evaluations.Delete;
using EMPLOYEE.Application.Features.Evaluations.EvaluationDtos;
using EMPLOYEE.Application.Features.Evaluations.Update;
using EmployeeDomain.Entities;

namespace EMPLOYEE.Application.Features.Evaluations.EvaluationService
{
    public class EvaluationAppService : IEvaluationAppService
    {
        private readonly CreateEvaluationService _create;
        private readonly UpdateEvaluationService _update;
        private readonly DeleteEvaluationService _delete;
      


        public EvaluationAppService(
            CreateEvaluationService create,
            UpdateEvaluationService update,
            DeleteEvaluationService delete
                                            )
        {
            _create = create;
            _update = update;
            _delete = delete;
          
        }

        public Task<Result<string>> CreateAsync(
         CreateEvaluationDto dto,
         string? callerId,
         bool isAdmin)
        {
           
                return _create.HandleAsync(dto, callerId!, isAdmin);
        }

        public Task<Result<string>> UpdateAsync(

              EmployeeEvaluationByPeriodDto dto,
              UpdateEvaluationDto updateDto,
            string callerId,
            bool isAdmin)
            => _update.HandleAsync(dto, updateDto, callerId, isAdmin);

        public Task<Result<string>> DeleteAsync(
   EmployeeEvaluationByPeriodDto dto,
      string callerId,
      bool isAdmin)
       => _delete.HandleAsync(dto, callerId, isAdmin);
    }

}
