using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Prometheus;
namespace EnterpriseIDP.API.Observability;
public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceName = "enterprise-idp-backend";
        var serviceVersion = "1.0.0";
        var otlpEndpoint = configuration["Observability:OtlpEndpoint"];
        var hasOtlp = !string.IsNullOrWhiteSpace(otlpEndpoint);
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] =
                        configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development",
                    ["team"] = "platform-engineering"
                }))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(opts =>
                    {
                        opts.RecordException = true;
                        opts.Filter = ctx =>
                            !ctx.Request.Path.StartsWithSegments("/health") &&
                            !ctx.Request.Path.StartsWithSegments("/metrics");
                    })
                    .AddHttpClientInstrumentation(opts => opts.RecordException = true);
                if (hasOtlp)
                    tracing.AddOtlpExporter(opts => opts.Endpoint = new Uri(otlpEndpoint!));
            })
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter()
                .AddOtlpExporter(opts => { if (!string.IsNullOrWhiteSpace(otlpEndpoint)) { opts.Endpoint = new Uri(otlpEndpoint); opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf; } }));
        return services;
    }
    public static IApplicationBuilder UseObservability(
        this IApplicationBuilder app)
    {
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
        app.UseHttpMetrics(options =>
        {
            options.AddCustomLabel("service", _ => "enterprise-idp");
        });
        return app;
    }
}
