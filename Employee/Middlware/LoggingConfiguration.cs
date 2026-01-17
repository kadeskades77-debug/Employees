namespace EMPLOYEE.Middlware
{
  using Serilog;

public static class LoggingConfiguration
{
    public static void AddSerilogConfig(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}

}
