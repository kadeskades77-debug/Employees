using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Serilog;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public class BusinessException : Exception
{
    public int StatusCode { get; }

    public BusinessException(string message, int statusCode = 403) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class SlackExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _slackWebhookUrl;

    public SlackExceptionMiddleware(RequestDelegate next, string slackWebhookUrl)
    {
        _next = next;
        _slackWebhookUrl = slackWebhookUrl;
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
            await SendToSlack(context, ex);

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
                    message = "حدث خطأ في الخادم";
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

    private async Task SendToSlack(HttpContext context, Exception ex)
    {
        using var client = new HttpClient();
        var requestInfo = await GetRequestInfoAsync(context);

        var routeData = context.GetRouteData();
        string controller = routeData?.Values["controller"]?.ToString() ?? "UnknownController";
        string action = routeData?.Values["action"]?.ToString() ?? "UnknownAction";

        string color = GetErrorColor(ex);

        var payload = new
        {
            attachments = new[]
            {
                new
                {
                    color = color,
                    title = $"🚨 Exception in {controller}/{action}",
                    text = $"Message: {ex.Message}\nStackTrace:\n{ex.StackTrace}\nRequest Info:\n{requestInfo}"
                }
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await client.PostAsync(_slackWebhookUrl, content);
    }

    private string GetErrorColor(Exception ex)
    {
        return ex switch
        {
            ArgumentNullException => "warning",
            UnauthorizedAccessException => "warning",
            BusinessException => "warning",
            InvalidOperationException => "warning",
            _ => "danger"
        };
    }

    private async Task<string> GetRequestInfoAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        string body = "";

        if (context.Request.ContentLength > 0)
        {
            context.Request.Body.Position = 0;
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (body.Length > 1000)
                body = body.Substring(0, 1000) + "...(truncated)";

            body = HideSensitiveData(body);
        }

        var info = new
        {
            Method = context.Request.Method,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            Body = body,
            User = context.User?.Identity?.Name
        };

        return JsonSerializer.Serialize(info);
    }

    private string HideSensitiveData(string body)
    {
        body = Regex.Replace(
            body,
            "(\"password\"|\"token\"|\"pwd\")\\s*:\\s*\".*?\"",
            "$1: \"***hidden***\"",
            RegexOptions.IgnoreCase
        );
        return body;
    }
}
