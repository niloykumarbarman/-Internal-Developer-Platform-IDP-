using MediatR;

namespace EnterpriseIDP.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<CurrentUserDto>;

public record CurrentUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    DateTime CreatedAt
);
