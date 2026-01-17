namespace EMPLOYEE.Middlware
{
    public class BusinessException : Exception
    {
        public int StatusCode { get; }

        public BusinessException(string message, int statusCode = 403) : base(message)
        {
            StatusCode = statusCode;
        }
    }

}
