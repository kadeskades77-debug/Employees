namespace EMPLOYEE.Application.Features.Evaluations.EvaluationDtos
{
    public class EmployeeEvaluationsResponseDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;

        public string ManagerName { get; set; } = string.Empty;

        public List<EvaluationItemDto> Evaluations { get; set; } = new();
    }

}
