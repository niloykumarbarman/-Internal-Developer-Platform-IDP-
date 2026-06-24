using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.Catalog.Queries.GetServiceById;

public record GetServiceByIdQuery(Guid Id) : IRequest<ServiceDetailDto>;

public record ServiceDetailDto(
    Guid Id, string Name, string Slug, string Description,
    ServiceType Type, ServiceStatus Status, string Owner,
    Guid TeamId, string? RepositoryUrl, string? DocumentationUrl,
    List<string> Tags, List<DependencyDto> Dependencies, DateTime CreatedAt, DateTime UpdatedAt
);

public record DependencyDto(Guid ServiceId, string ServiceName, string DependencyType);
