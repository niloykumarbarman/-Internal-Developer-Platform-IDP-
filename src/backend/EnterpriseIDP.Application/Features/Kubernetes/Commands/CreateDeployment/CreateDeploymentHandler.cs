using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Application.Common.Extensions;
using EnterpriseIDP.Application.Common.Interfaces;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Catalog;
using EnterpriseIDP.Domain.Entities.Kubernetes;
using MediatR;

namespace EnterpriseIDP.Application.Features.Kubernetes.Commands.CreateDeployment;

public class CreateDeploymentHandler : IRequestHandler<CreateDeploymentCommand, CreateDeploymentResult>
{
    private readonly IRepository<KubernetesDeployment> _deploymentRepo;
    private readonly IRepository<KubernetesNamespace> _nsRepo;
    private readonly IRepository<Service> _serviceRepo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreateDeploymentHandler(IRepository<KubernetesDeployment> deploymentRepo,
        IRepository<KubernetesNamespace> nsRepo, IRepository<Service> serviceRepo,
        IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _deploymentRepo = deploymentRepo;
        _nsRepo = nsRepo;
        _serviceRepo = serviceRepo;
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<CreateDeploymentResult> Handle(CreateDeploymentCommand cmd, CancellationToken ct)
    {
        _ = await _serviceRepo.GetByIdAsync(cmd.ServiceId, ct)
            ?? throw new NotFoundException(nameof(Service), cmd.ServiceId);

        _ = await _nsRepo.GetByIdAsync(cmd.NamespaceId, ct)
            ?? throw new NotFoundException(nameof(KubernetesNamespace), cmd.NamespaceId);

        var deployment = KubernetesDeployment.Create(
            cmd.Name,
            cmd.ServiceId,
            cmd.NamespaceId,
            cmd.ImageName,
            cmd.ImageTag,
            cmd.Replicas,
            cmd.Environment,
            _currentUser.UserId?.ToString() ?? "system"
        ).ThrowIfError();

        await _deploymentRepo.AddAsync(deployment, ct);
        await _uow.SaveChangesAsync(ct);

        return new CreateDeploymentResult(
            deployment.Id, deployment.Name, deployment.ServiceId, deployment.NamespaceId,
            deployment.ImageName, deployment.ImageTag, deployment.Replicas,
            deployment.Status, deployment.Environment, deployment.CreatedAt
        );
    }
}
