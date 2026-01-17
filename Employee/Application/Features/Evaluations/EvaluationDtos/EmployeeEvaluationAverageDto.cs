namespace EMPLOYEE.Application.Features.Evaluations.EvaluationDtos
{
    public class EmployeeEvaluationAverageDto
    {
        public string PeriodLabel { get; set; } = string.Empty;
        public decimal Attendance { get; set; }
        public decimal Quality { get; set; }
        public decimal Productivity { get; set; }
        public decimal Teamwork { get; set; }
        public decimal FinalScore { get; set; }
        public string Rating { get; set; } = string.Empty;
    }

}
