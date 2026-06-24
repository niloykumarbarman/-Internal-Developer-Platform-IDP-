using MediatR;

namespace EnterpriseIDP.Application.Features.Auth.Commands.RegisterUser;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role = "Developer"
) : IRequest<RegisterUserResult>;

public record RegisterUserResult(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string Token
);
