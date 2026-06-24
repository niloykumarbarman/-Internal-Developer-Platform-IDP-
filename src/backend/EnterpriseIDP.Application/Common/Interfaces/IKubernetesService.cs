namespace EnterpriseIDP.Application.Common.Interfaces;

public interface IKubernetesService
{
    Task<bool> CreateNamespaceAsync(string namespaceName, Dictionary<string, string> labels, CancellationToken ct = default);
    Task<bool> NamespaceExistsAsync(string namespaceName, CancellationToken ct = default);
    Task<IEnumerable<string>> GetNamespacesAsync(CancellationToken ct = default);
    Task ApplyResourceQuotaAsync(string namespaceName, string cpuLimit, string memoryLimit, CancellationToken ct = default);
}
