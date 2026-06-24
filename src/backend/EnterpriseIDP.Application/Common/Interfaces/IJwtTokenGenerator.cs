using EnterpriseIDP.Application.Features.Auth.Commands.LoginUser;

namespace EnterpriseIDP.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, string role);
}
