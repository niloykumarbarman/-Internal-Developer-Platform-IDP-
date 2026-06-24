namespace EnterpriseIDP.Application.Interfaces;

public interface IHttpContextAccessorWrapper
{
    string? GetIpAddress();
    string? GetUserAgent();
    string? GetRequestPath();
}
