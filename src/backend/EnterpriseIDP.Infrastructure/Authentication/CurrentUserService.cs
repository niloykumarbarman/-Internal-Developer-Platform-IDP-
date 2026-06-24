using System.Security.Claims;
using EnterpriseIDP.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EnterpriseIDP.Infrastructure.Authentication;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User?.FindFirstValue("sub");

    public string? UserName => User?.FindFirstValue(ClaimTypes.Name)
        ?? User?.FindFirstValue("name");

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    public string? Role => User?.FindFirstValue(ClaimTypes.Role);

    public IEnumerable<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
