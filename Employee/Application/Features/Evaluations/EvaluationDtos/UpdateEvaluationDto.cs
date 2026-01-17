namespace EMPLOYEE.Application.Features.Evaluations.EvaluationDtos
{
    public class UpdateEvaluationDto
    {
     
        public int Attendance { get; set; }
        public int Quality { get; set; }
        public int Productivity { get; set; }
        public int Teamwork { get; set; }
        public string? Comments { get; set; }
    }
}
