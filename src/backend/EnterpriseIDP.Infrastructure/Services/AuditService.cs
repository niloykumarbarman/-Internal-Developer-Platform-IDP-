using EnterpriseIDP.Application.Common.Interfaces;
using EnterpriseIDP.Application.Interfaces;
using EnterpriseIDP.Domain.Entities;
using EnterpriseIDP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnterpriseIDP.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IHttpContextAccessorWrapper _httpContextAccessor;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        ApplicationDbContext context,
        ICurrentUserService currentUser,
        IHttpContextAccessorWrapper httpContextAccessor,
        ILogger<AuditService> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(
        string action,
        string entityType,
        string? entityId = null,
        string? oldValues = null,
        string? newValues = null,
        string? additionalInfo = null,
        bool isSuccess = true,
        string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                UserId = _currentUser.UserId ?? "system",
                UserName = _currentUser.UserName ?? "system",
                IpAddress = _httpContextAccessor.GetIpAddress(),
                OldValues = oldValues,
                NewValues = newValues,
                AdditionalInfo = additionalInfo,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write audit log: {Action} {EntityType}", action, entityType);
        }
    }

    public async Task<List<AuditLog>> GetLogsAsync(
        string? userId = null,
        string? entityType = null,
        string? action = null,
        DateTime? from = null,
        DateTime? to = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
            query = query.Where(l => l.UserId == userId);

        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(l => l.EntityType == entityType);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(l => l.Action == action);

        if (from.HasValue)
            query = query.Where(l => l.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(l => l.CreatedAt <= to.Value);

        return await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? userId = null,
        string? entityType = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
            query = query.Where(l => l.UserId == userId);

        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(l => l.EntityType == entityType);

        if (from.HasValue)
            query = query.Where(l => l.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(l => l.CreatedAt <= to.Value);

        return await query.CountAsync(cancellationToken);
    }
}
