using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.Catalog.Commands.RegisterService;

public record RegisterServiceCommand(
    string Name,
    string Description,
    ServiceType Type,
    string Owner,
    Guid TeamId,
    string? RepositoryUrl,
    string? DocumentationUrl,
    List<string> Tags
) : IRequest<RegisterServiceResult>;

public record RegisterServiceResult(
    Guid Id,
    string Name,
    string Slug,
    ServiceType Type,
    string Owner,
    ServiceStatus Status,
    DateTime CreatedAt
);
