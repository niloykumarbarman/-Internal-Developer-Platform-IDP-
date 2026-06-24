using MediatR;

namespace EnterpriseIDP.Application.Features.Auth.Queries.GetTeams;

public record GetTeamsQuery : IRequest<List<TeamDto>>;

public record TeamDto(
    Guid Id,
    string Name,
    string Description,
    Guid OwnerId,
    int MemberCount,
    DateTime CreatedAt
);
