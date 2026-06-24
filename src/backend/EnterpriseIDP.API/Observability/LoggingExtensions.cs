using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace EnterpriseIDP.API.Observability;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddStructuredLogging(
        this WebApplicationBuilder builder)
    {
        var lokiUrl = builder.Configuration["Observability:LokiUrl"]
            ?? "http://localhost:3100";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.WithProperty("Application", "enterprise-idp-backend")
            .Enrich.WithProperty("Version", "1.0.0")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] " +
                    "{Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.GrafanaLoki(
                lokiUrl,
                labels:
                [
                    new LokiLabel { Key = "app", Value = "enterprise-idp" },
                    new LokiLabel { Key = "env", Value =
                        builder.Environment.EnvironmentName.ToLower() }
                ],
                propertiesAsLabels: ["level", "RequestPath"])
            .CreateLogger();

        builder.Host.UseSerilog();
        return builder;
    }
}
