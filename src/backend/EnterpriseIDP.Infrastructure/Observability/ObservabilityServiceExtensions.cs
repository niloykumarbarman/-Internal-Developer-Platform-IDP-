using EnterpriseIDP.Infrastructure.Observability.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseIDP.Infrastructure.Observability;

public static class ObservabilityServiceExtensions
{
    public static IServiceCollection AddPrometheusObservability(this IServiceCollection services)
    {
        return services;
    }

    public static IApplicationBuilder UsePrometheusObservability(this IApplicationBuilder app)
    {
        app.UseMiddleware<PrometheusMetricsMiddleware>();
        return app;
    }
}
