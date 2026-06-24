using System.Diagnostics;
using EnterpriseIDP.Infrastructure.Observability.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace EnterpriseIDP.Infrastructure.Observability.Middleware;

public class PrometheusMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PrometheusMetricsMiddleware> _logger;

    public PrometheusMetricsMiddleware(RequestDelegate next, ILogger<PrometheusMetricsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/metrics"))
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();
        var method = context.Request.Method;
        var endpoint = GetNormalizedEndpoint(context);

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            var statusCode = context.Response.StatusCode.ToString();
            IDPMetrics.HttpRequestsTotal.WithLabels(method, endpoint, statusCode).Inc();
            IDPMetrics.HttpRequestDuration.WithLabels(method, endpoint).Observe(sw.Elapsed.TotalSeconds);
        }
    }

    private static string GetNormalizedEndpoint(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is RouteEndpoint routeEndpoint)
            return routeEndpoint.RoutePattern.RawText ?? context.Request.Path;
        return context.Request.Path.Value ?? "unknown";
    }
}
