namespace EMPLOYEE.Application.Features.Evaluations.EvaluationDtos
{
    public class EmployeeYearlyDashboardDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public List<EmployeeEvaluationAverageDto?> Quarterly { get; set; } = new();
        public EmployeeEvaluationAverageDto? Yearly { get; set; } = new();

    }

}
