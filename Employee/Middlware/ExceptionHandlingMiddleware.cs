using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

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

            Log.Error(ex, "Unhandled Exception");

            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            switch (ex)
            {
                case BusinessException bex:
                    statusCode = bex.StatusCode;
                    message = bex.Message;
                    break;

                case UnauthorizedAccessException uex:
                    statusCode = 403;
                    message = uex.Message;
                    break;

                default:
                    statusCode = 500;
                    message = ex.Message;
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new
            {
                StatusCode = statusCode,
                Message = message
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
    }

