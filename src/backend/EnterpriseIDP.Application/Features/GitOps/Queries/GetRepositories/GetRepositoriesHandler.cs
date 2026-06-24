using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.GitOps;
using MediatR;

namespace EnterpriseIDP.Application.Features.GitOps.Queries.GetRepositories;

public class GetRepositoriesHandler : IRequestHandler<GetRepositoriesQuery, List<RepositoryDto>>
{
    private readonly IRepository<Repository> _repoRepo;

    public GetRepositoriesHandler(IRepository<Repository> repoRepo) => _repoRepo = repoRepo;

    public async Task<List<RepositoryDto>> Handle(GetRepositoriesQuery query, CancellationToken ct)
    {
        var all = await _repoRepo.GetAllAsync(ct);
        return all
            .Where(r => (!query.TeamId.HasValue || r.TeamId == query.TeamId)
                     && (!query.ServiceId.HasValue || r.ServiceId == query.ServiceId))
            .Select(r => new RepositoryDto(r.Id, r.Name, r.FullName, r.CloneUrl, r.DefaultBranch, r.ServiceId, r.TeamId, r.CreatedAt))
            .ToList();
    }
}
