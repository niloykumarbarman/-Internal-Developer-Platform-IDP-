using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Catalog;
using MediatR;

namespace EnterpriseIDP.Application.Features.Catalog.Queries.GetServices;

public class GetServicesHandler : IRequestHandler<GetServicesQuery, GetServicesResult>
{
    private readonly IRepository<Service> _serviceRepo;

    public GetServicesHandler(IRepository<Service> serviceRepo) => _serviceRepo = serviceRepo;

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
        var items = filtered
            .OrderByDescending(s => s.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => new ServiceListDto(
                s.Id, s.Name, s.Slug.Value, s.Description ?? string.Empty, s.Type,
                s.Status, s.OwnerUserId.ToString(), s.RepositoryUrl,
                s.Tags.Select(t => t.Value).ToList(), s.CreatedAt))
            .ToList();

        return new GetServicesResult(items, total, query.Page, query.PageSize);
    }
}
