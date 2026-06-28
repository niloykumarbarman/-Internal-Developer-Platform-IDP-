using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Catalog;
using EnterpriseIDP.Domain.Entities.Auth;
using MediatR;

namespace EnterpriseIDP.Application.Features.Catalog.Queries.GetServices;

public class GetServicesHandler : IRequestHandler<GetServicesQuery, GetServicesResult>
{
    private readonly IRepository<Service> _serviceRepo;
    private readonly IRepository<Team> _teamRepo;

    public GetServicesHandler(IRepository<Service> serviceRepo, IRepository<Team> teamRepo)
    {
        _serviceRepo = serviceRepo;
        _teamRepo = teamRepo;
    }

    public async Task<GetServicesResult> Handle(GetServicesQuery query, CancellationToken ct)
    {
        var all = await _serviceRepo.GetAllAsync(ct);

        var filtered = all.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Search))
            filtered = filtered.Where(s => s.Name.Contains(query.Search, StringComparison.OrdinalIgnoreCase)
                                        || (s.Description ?? string.Empty).Contains(query.Search, StringComparison.OrdinalIgnoreCase));
        if (query.Type.HasValue)    filtered = filtered.Where(s => s.Type == query.Type.Value);
        if (query.Status.HasValue)  filtered = filtered.Where(s => s.Status == query.Status.Value);
        if (query.TeamId.HasValue)  filtered = filtered.Where(s => s.OwnerTeamId == query.TeamId.Value);

        var total = filtered.Count();
        var pagedServices = filtered
            .OrderByDescending(s => s.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var teamIds = pagedServices.Select(s => s.OwnerTeamId).Distinct().ToList();
        var teams = await _teamRepo.FindAsync(t => teamIds.Contains(t.Id), ct);
        var teamNameById = teams.ToDictionary(t => t.Id, t => t.Name);

        var items = pagedServices
            .Select(s => new ServiceListDto(
                s.Id, s.Name, s.Slug.Value, s.Description ?? string.Empty, s.Type,
                s.Status, s.OwnerUserId.ToString(),
                teamNameById.TryGetValue(s.OwnerTeamId, out var teamName) ? teamName : "Unknown Team",
                s.RepositoryUrl,
                s.Tags.Select(t => t.Value).ToList(), s.CreatedAt))
            .ToList();

        return new GetServicesResult(items, total, query.Page, query.PageSize);
    }
}
