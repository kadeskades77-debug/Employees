namespace EMPLOYEE.Application.Features.Evaluations.EvaluationDtos
{
    public class EmployeeEvaluationByPeriodDto
    {
        public int EmployeeId { get; set; }
        public string PeriodText { get; set; }//yyyy-MM
        public DateTime Period
        {
            get
            {
                if (DateTime.TryParseExact(PeriodText, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out var dt))
                    return new DateTime(dt.Year, dt.Month, 1);
                throw new ArgumentException("Invalid Period format. Use yyyy-MM.");
            }
        }
    }
}
