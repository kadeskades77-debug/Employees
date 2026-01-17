namespace EMPLOYEE.Application.Common
{
    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool success, T? data, string? error)
            : base(success, error)
        {
            Data = data;
        }

        public static Result<T> Ok(T data)
            => new Result<T>(true, data, null);

        public static Result<T> Fail(string error)
            => new Result<T>(false, default, error);
    }
}
