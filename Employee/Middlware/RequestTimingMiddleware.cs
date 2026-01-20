using System.Diagnostics;

namespace EMPLOYEE.Middlware
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(
            RequestDelegate next,
            ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            await _next(context);

            sw.Stop();

            _logger.LogInformation(
                "Request {Method} {Path} executed in {Elapsed} ms",
                context.Request.Method,
                context.Request.Path,
                sw.ElapsedMilliseconds);
        }
    }


}


