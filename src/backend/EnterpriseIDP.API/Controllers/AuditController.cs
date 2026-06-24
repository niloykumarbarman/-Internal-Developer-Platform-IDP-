using EnterpriseIDP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseIDP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,PlatformEngineer")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditController> _logger;

    public AuditController(IAuditService auditService, ILogger<AuditController> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    /// <summary>Get audit logs with filtering</summary>
    [HttpGet]
    public async Task<IActionResult> GetLogs(
        [FromQuery] string? userId,
        [FromQuery] string? entityType,
        [FromQuery] string? action,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var logs = await _auditService.GetLogsAsync(
            userId, entityType, action, from, to, page, pageSize);

        var total = await _auditService.GetTotalCountAsync(
            userId, entityType, from, to);

        return Ok(new
        {
            items = logs,
            totalCount = total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }

    /// <summary>Get audit logs for a specific entity</summary>
    [HttpGet("{entityType}/{entityId}")]
    public async Task<IActionResult> GetEntityLogs(
        string entityType,
        string entityId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var logs = await _auditService.GetLogsAsync(
            entityType: entityType,
            page: page,
            pageSize: pageSize);

        var filtered = logs.Where(l => l.EntityId == entityId).ToList();
        return Ok(new { items = filtered, count = filtered.Count });
    }

    /// <summary>Get audit summary stats</summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow;

        var logs = await _auditService.GetLogsAsync(
            from: fromDate, to: toDate, pageSize: 1000);

        var stats = new
        {
            totalActions = logs.Count,
            successCount = logs.Count(l => l.IsSuccess),
            failureCount = logs.Count(l => !l.IsSuccess),
            byAction = logs.GroupBy(l => l.Action)
                .Select(g => new { action = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count),
            byEntityType = logs.GroupBy(l => l.EntityType)
                .Select(g => new { entityType = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count),
            byUser = logs.GroupBy(l => l.UserName)
                .Select(g => new { user = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .Take(10),
            period = new { from = fromDate, to = toDate }
        };

        return Ok(stats);
    }
}
