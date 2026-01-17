namespace EMPLOYEE.Application.Features.Evaluations.EvaluationDtos
{
    public class EvaluationItemDto
    {

        public int Attendance { get; set; }
        public int Quality { get; set; }
        public int Productivity { get; set; }
        public int Teamwork { get; set; }

        public string Comments { get; set; } = string.Empty;
        public DateTime Period { get; set; } 
        public decimal FinalScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
