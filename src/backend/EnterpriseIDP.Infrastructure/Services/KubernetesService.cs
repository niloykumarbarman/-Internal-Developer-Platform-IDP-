using EnterpriseIDP.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace EnterpriseIDP.Infrastructure.Services;

/// <summary>
/// Simulated Kubernetes service for Phase 1 (no live cluster dependency).
/// Tracks namespaces in-memory so the platform flow can be demoed end-to-end.
/// Replace with a real client (KubernetesClient / k8s SDK) when a cluster is wired up.
/// </summary>
public class KubernetesService : IKubernetesService
{
    private readonly ILogger<KubernetesService> _logger;
    private static readonly HashSet<string> _namespaces = new();

    public KubernetesService(ILogger<KubernetesService> logger)
    {
        _logger = logger;
    }

    public Task<bool> CreateNamespaceAsync(string namespaceName, Dictionary<string, string> labels, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating namespace '{Namespace}' with labels: {Labels}",
            namespaceName, string.Join(", ", labels.Select(l => $"{l.Key}={l.Value}")));

        _namespaces.Add(namespaceName);
        return Task.FromResult(true);
    }

    public Task<bool> NamespaceExistsAsync(string namespaceName, CancellationToken ct = default)
        => Task.FromResult(_namespaces.Contains(namespaceName));

    public Task<IEnumerable<string>> GetNamespacesAsync(CancellationToken ct = default)
        => Task.FromResult<IEnumerable<string>>(_namespaces.ToList());

    public Task ApplyResourceQuotaAsync(string namespaceName, string cpuLimit, string memoryLimit, CancellationToken ct = default)
    {
        _logger.LogInformation("Applying resource quota to '{Namespace}': CPU={Cpu}, Memory={Memory}",
            namespaceName, cpuLimit, memoryLimit);
        return Task.CompletedTask;
    }
}
