using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Kubernetes;
using MediatR;

namespace EnterpriseIDP.Application.Features.Kubernetes.Queries.GetDeployments;

public class GetDeploymentsHandler : IRequestHandler<GetDeploymentsQuery, List<DeploymentDto>>
{
    private readonly IRepository<KubernetesDeployment> _deployRepo;

    public GetDeploymentsHandler(IRepository<KubernetesDeployment> deployRepo) => _deployRepo = deployRepo;

    public async Task<List<DeploymentDto>> Handle(GetDeploymentsQuery query, CancellationToken ct)
    {
        var all = await _deployRepo.GetAllAsync(ct);
        return all
            .Where(d => (!query.ServiceId.HasValue || d.ServiceId == query.ServiceId)
                     && (!query.Environment.HasValue || d.Environment == query.Environment))
            .Select(d => new DeploymentDto(
                d.Id, d.Name, d.Namespace != null ? d.Namespace.Name : string.Empty,
                d.ImageName, d.ImageTag,
                d.Replicas, d.Status, d.Environment, d.ServiceId, d.CreatedAt))
            .ToList();
    }
}
