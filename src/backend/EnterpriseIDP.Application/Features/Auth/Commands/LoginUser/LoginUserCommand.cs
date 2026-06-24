using MediatR;

namespace EnterpriseIDP.Application.Features.Auth.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResult>;

public record LoginUserResult(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string Token
);
