namespace EMPLOYEE.Application.Features.Evaluations.EvaluationDtos
{
    public class EvaluationDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;


        public int Attendance { get; set; }
        public int Quality { get; set; }
        public int Productivity { get; set; }
        public int Teamwork { get; set; }


        public string? Comments { get; set; } = string.Empty;
        public decimal FinalScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
