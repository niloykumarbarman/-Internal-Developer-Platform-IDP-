using EnterpriseIDP.Domain.Entities;

namespace EnterpriseIDP.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string entityType, string? entityId = null,
        string? oldValues = null, string? newValues = null,
        string? additionalInfo = null, bool isSuccess = true,
        string? errorMessage = null, CancellationToken cancellationToken = default);

    Task<List<AuditLog>> GetLogsAsync(
        string? userId = null,
        string? entityType = null,
        string? action = null,
        DateTime? from = null,
        DateTime? to = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    Task<int> GetTotalCountAsync(
        string? userId = null,
        string? entityType = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
}
