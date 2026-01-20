using EMPLOYEE.Middlware;
using Serilog;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
                throw;

            int statusCode;
            string message;

            switch (ex)
            {
                case BusinessException bex:
                    statusCode = bex.StatusCode;
                    message = bex.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = "Unauthorized access.";
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = _env.IsDevelopment()
                        ? ex.Message
                        : "An unexpected error occurred.";
                    break;
            }

            Log.Error(ex,
                "Unhandled exception. Path: {Path}, StatusCode: {StatusCode}",
                context.Request.Path,
                statusCode);

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
