namespace EnterpriseIDP.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    string? Role { get; }
    IEnumerable<string> Roles { get; }
    bool IsInRole(string role);
    bool IsAuthenticated { get; }
}
