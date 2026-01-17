namespace EMPLOYEE.Models;

public class EmployeeEvaluationRead
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public string ManagerId { get; set; } = null!;
    public ApplicationUser Manager { get; set; } = null!;

    public int Attendance { get; set; }
    public int Quality { get; set; }
    public int Productivity { get; set; }
    public int Teamwork { get; set; }

    public decimal FinalScore { get; set; }
    public string? Comments { get; set; }

    public DateTime Period { get; set; }
    public DateTime CreatedAt { get; set; }
}
