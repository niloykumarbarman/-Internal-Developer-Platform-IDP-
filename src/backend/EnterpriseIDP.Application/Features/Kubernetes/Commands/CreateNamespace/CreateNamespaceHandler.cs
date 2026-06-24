using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Application.Common.Extensions;
using EnterpriseIDP.Application.Common.Interfaces;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Auth;
using EnterpriseIDP.Domain.Entities.Kubernetes;
using MediatR;

namespace EnterpriseIDP.Application.Features.Kubernetes.Commands.CreateNamespace;

public class CreateNamespaceHandler : IRequestHandler<CreateNamespaceCommand, CreateNamespaceResult>
{
    private readonly IRepository<KubernetesNamespace> _nsRepo;
    private readonly IRepository<Team> _teamRepo;
    private readonly IKubernetesService _k8s;
    private readonly IUnitOfWork _uow;

    public CreateNamespaceHandler(IRepository<KubernetesNamespace> nsRepo, IRepository<Team> teamRepo,
        IKubernetesService k8s, IUnitOfWork uow)
    {
        _nsRepo = nsRepo;
        _teamRepo = teamRepo;
        _k8s = k8s;
        _uow = uow;
    }

    public async Task<CreateNamespaceResult> Handle(CreateNamespaceCommand cmd, CancellationToken ct)
    {
        var team = await _teamRepo.GetByIdAsync(cmd.TeamId, ct)
            ?? throw new NotFoundException(nameof(Team), cmd.TeamId);

        if (await _k8s.NamespaceExistsAsync(cmd.Name, ct))
            throw new ConflictException($"Namespace '{cmd.Name}' already exists.");

        var labels = new Dictionary<string, string>
        {
            ["team"] = team.Name.ToLower().Replace(" ", "-"),
            ["environment"] = cmd.Environment.ToString().ToLower(),
            ["managed-by"] = "enterprise-idp"
        };

        await _k8s.CreateNamespaceAsync(cmd.Name, labels, ct);
        await _k8s.ApplyResourceQuotaAsync(cmd.Name, cmd.CpuLimit, cmd.MemoryLimit, ct);

        var ns = KubernetesNamespace.Create(cmd.Name, cmd.Environment, cmd.TeamId, null, team.OwnerId.ToString())
            .ThrowIfError();

        await _nsRepo.AddAsync(ns, ct);
        await _uow.SaveChangesAsync(ct);

        return new CreateNamespaceResult(ns.Id, ns.Name, ns.TeamId, ns.Environment, ns.CreatedAt);
    }
}
