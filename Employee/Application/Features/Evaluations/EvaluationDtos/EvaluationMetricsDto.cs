using EMPLOYEE.Application.Common.Interfaces;

public class EvaluationMetricsDto : IEvaluationMetrics
{
    public int Attendance { get; set; }
    public int Quality { get; set; }
    public int Productivity { get; set; }
    public int Teamwork { get; set; }
    public decimal FinalScore { get; set; }
    public DateTime Period { get; set; }
}
