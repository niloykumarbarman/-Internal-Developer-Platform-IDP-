using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Catalog;
using MediatR;

namespace EnterpriseIDP.Application.Features.Catalog.Queries.GetServiceById;

public class GetServiceByIdHandler : IRequestHandler<GetServiceByIdQuery, ServiceDetailDto>
{
    private readonly IRepository<Service> _serviceRepo;

    public GetServiceByIdHandler(IRepository<Service> serviceRepo) => _serviceRepo = serviceRepo;

    public async Task<ServiceDetailDto> Handle(GetServiceByIdQuery query, CancellationToken ct)
    {
        var s = await _serviceRepo.GetByIdAsync(query.Id, ct)
            ?? throw new NotFoundException(nameof(Service), query.Id);

        return new ServiceDetailDto(
            s.Id, s.Name, s.Slug.Value, s.Description ?? string.Empty, s.Type, s.Status,
            s.OwnerUserId.ToString(), s.OwnerTeamId, s.RepositoryUrl, s.DocumentationUrl,
            s.Tags.Select(t => t.Value).ToList(),
            s.Dependencies.Select(d => new DependencyDto(d.DependsOnServiceId, d.DependsOnServiceId.ToString(), "DependsOn")).ToList(),
            s.CreatedAt, s.UpdatedAt ?? s.CreatedAt
        );
    }
}
