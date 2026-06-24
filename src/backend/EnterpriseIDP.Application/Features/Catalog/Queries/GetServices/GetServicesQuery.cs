using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.Catalog.Queries.GetServices;

public record GetServicesQuery(
    string? Search = null,
    ServiceType? Type = null,
    ServiceStatus? Status = null,
    Guid? TeamId = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<GetServicesResult>;

public record GetServicesResult(List<ServiceListDto> Items, int TotalCount, int Page, int PageSize);

public record ServiceListDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    ServiceType Type,
    ServiceStatus Status,
    string Owner,
    string? RepositoryUrl,
    List<string> Tags,
    DateTime CreatedAt
);
