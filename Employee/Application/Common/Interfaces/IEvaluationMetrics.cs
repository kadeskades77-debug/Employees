namespace EMPLOYEE.Application.Common.Interfaces
{
    public interface IEvaluationMetrics
    {
        int Attendance { get; }
        int Quality { get; }
        int Productivity { get; }
        int Teamwork { get; }
        decimal FinalScore { get; }
        DateTime Period { get; }
    }
}
