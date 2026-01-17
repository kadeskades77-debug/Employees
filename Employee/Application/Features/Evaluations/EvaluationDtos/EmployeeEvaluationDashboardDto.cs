namespace EMPLOYEE.Application.Features.Evaluations.EvaluationDtos
{
    public class EmployeeEvaluationDashboardDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public List<EmployeeEvaluationAverageDto> Monthly { get; set; } = new();
        public EmployeeEvaluationAverageDto? Yearly { get; set; }

    }

}
