using EnterpriseIDP.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EnterpriseIDP.Infrastructure.Services;

public class HttpContextAccessorWrapper : IHttpContextAccessorWrapper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextAccessorWrapper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext? Context => _httpContextAccessor.HttpContext;

    public string? GetIpAddress()
    {
        var context = Context;
        if (context == null) return null;

        // Check for forwarded IP (behind proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
            return forwardedFor.Split(',').First().Trim();

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;

        return context.Connection.RemoteIpAddress?.ToString();
    }

    public string? GetUserAgent()
    {
        return Context?.Request.Headers["User-Agent"].FirstOrDefault();
    }

    public string? GetRequestPath()
    {
        return Context?.Request.Path.ToString();
    }
}
