using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Auth;
using MediatR;

namespace EnterpriseIDP.Application.Features.Auth.Queries.GetTeams;

public class GetTeamsHandler : IRequestHandler<GetTeamsQuery, List<TeamDto>>
{
    private readonly IRepository<Team> _teamRepo;

    public GetTeamsHandler(IRepository<Team> teamRepo) => _teamRepo = teamRepo;

    public async Task<List<TeamDto>> Handle(GetTeamsQuery query, CancellationToken ct)
    {
        var teams = await _teamRepo.GetAllAsync(ct);
        return teams.Select(t => new TeamDto(
            t.Id, t.Name, t.Description, t.OwnerId, t.Members.Count, t.CreatedAt
        )).ToList();
    }
}
