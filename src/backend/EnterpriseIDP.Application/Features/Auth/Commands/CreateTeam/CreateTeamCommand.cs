using MediatR;

namespace EnterpriseIDP.Application.Features.Auth.Commands.CreateTeam;

public record CreateTeamCommand(string Name, string Description, Guid OwnerId) : IRequest<CreateTeamResult>;
public record CreateTeamResult(Guid Id, string Name, string Description, Guid OwnerId);
