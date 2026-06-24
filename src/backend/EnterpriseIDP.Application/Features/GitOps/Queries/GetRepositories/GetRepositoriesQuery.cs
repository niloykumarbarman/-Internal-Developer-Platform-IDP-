using MediatR;

namespace EnterpriseIDP.Application.Features.GitOps.Queries.GetRepositories;

public record GetRepositoriesQuery(Guid? TeamId = null, Guid? ServiceId = null) : IRequest<List<RepositoryDto>>;

public record RepositoryDto(
    Guid Id, string Name, string FullName,
    string CloneUrl, string DefaultBranch,
    Guid ServiceId, Guid TeamId, DateTime CreatedAt
);
