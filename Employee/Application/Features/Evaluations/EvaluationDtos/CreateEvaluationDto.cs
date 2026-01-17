namespace EMPLOYEE.Application.Features.Evaluations.EvaluationDtos
{
    public class CreateEvaluationDto
    {
        public int EmployeeId { get; set; }
        public int Attendance { get; set; }      // 1..5
        public int Quality { get; set; }         // 1..5
        public int Productivity { get; set; }    // 1..5
        public int Teamwork { get; set; }        // 1..5
        public string? Comments { get; set; }
      
    }
}
