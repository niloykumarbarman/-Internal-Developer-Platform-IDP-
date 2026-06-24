using EnterpriseIDP.Application.Interfaces;
using EnterpriseIDP.Domain.Entities;
using EnterpriseIDP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseIDP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CostController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ILogger<CostController> _logger;

    public CostController(
        ApplicationDbContext context,
        IAuditService auditService,
        ILogger<CostController> logger)
    {
        _context = context;
        _auditService = auditService;
        _logger = logger;
    }

    /// <summary>Get cost reports with filtering</summary>
    [HttpGet("reports")]
    public async Task<IActionResult> GetCostReports(
        [FromQuery] string? teamName,
        [FromQuery] string? namespace_,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.CostReports
            .Include(r => r.ServiceCosts)
            .AsQueryable();

        if (!string.IsNullOrEmpty(teamName))
            query = query.Where(r => r.TeamName == teamName);

        if (!string.IsNullOrEmpty(namespace_))
            query = query.Where(r => r.Namespace == namespace_);

        if (from.HasValue)
            query = query.Where(r => r.PeriodStart >= from.Value);

        if (to.HasValue)
            query = query.Where(r => r.PeriodEnd <= to.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.GeneratedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { items, total, page, pageSize });
    }

    /// <summary>Get cost report by ID</summary>
    [HttpGet("reports/{id}")]
    public async Task<IActionResult> GetCostReport(Guid id)
    {
        var report = await _context.CostReports
            .Include(r => r.ServiceCosts)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (report == null)
            return NotFound(new { message = $"Cost report {id} not found" });

        return Ok(report);
    }

    /// <summary>Get cost summary by team</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetCostSummary([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var reports = await _context.CostReports
            .Where(r => r.PeriodStart >= fromDate && r.PeriodEnd <= toDate)
            .Include(r => r.ServiceCosts)
            .ToListAsync();

        var summary = new
        {
            totalCost = reports.Sum(r => r.TotalCost),
            cpuCost = reports.Sum(r => r.CpuCost),
            memoryCost = reports.Sum(r => r.MemoryCost),
            storageCost = reports.Sum(r => r.StorageCost),
            networkCost = reports.Sum(r => r.NetworkCost),
            byTeam = reports
                .GroupBy(r => r.TeamName)
                .Select(g => new
                {
                    team = g.Key,
                    totalCost = g.Sum(r => r.TotalCost),
                    reportCount = g.Count()
                })
                .OrderByDescending(x => x.totalCost),
            byNamespace = reports
                .GroupBy(r => r.Namespace)
                .Select(g => new
                {
                    namespace_ = g.Key,
                    totalCost = g.Sum(r => r.TotalCost)
                })
                .OrderByDescending(x => x.totalCost),
            period = new { from = fromDate, to = toDate }
        };

        return Ok(summary);
    }

    /// <summary>Create cost report manually</summary>
    [HttpPost("reports")]
    [Authorize(Roles = "Admin,PlatformEngineer")]
    public async Task<IActionResult> CreateCostReport([FromBody] CreateCostReportRequest request)
    {
        var report = new CostReport
        {
            TeamName = request.TeamName,
            Namespace = request.Namespace,
            Period = request.Period,
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            TotalCost = request.TotalCost,
            CpuCost = request.CpuCost,
            MemoryCost = request.MemoryCost,
            StorageCost = request.StorageCost,
            NetworkCost = request.NetworkCost,
            Currency = request.Currency
        };

        _context.CostReports.Add(report);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("CREATE", "CostReport", report.Id.ToString(),
            newValues: $"Team={report.TeamName}, Cost={report.TotalCost}");

        return CreatedAtAction(nameof(GetCostReport), new { id = report.Id }, report);
    }

    /// <summary>Get budget alerts</summary>
    [HttpGet("budgets")]
    public async Task<IActionResult> GetBudgetAlerts()
    {
        var alerts = await _context.BudgetAlerts
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return Ok(alerts);
    }

    /// <summary>Create budget alert</summary>
    [HttpPost("budgets")]
    [Authorize(Roles = "Admin,PlatformEngineer")]
    public async Task<IActionResult> CreateBudgetAlert([FromBody] CreateBudgetAlertRequest request)
    {
        var alert = new BudgetAlert
        {
            TeamName = request.TeamName,
            Namespace = request.Namespace,
            BudgetLimit = request.BudgetLimit,
            AlertThresholdPercent = request.AlertThresholdPercent,
            Currency = request.Currency
        };

        _context.BudgetAlerts.Add(alert);
        await _context.SaveChangesAsync();

        return Ok(alert);
    }

    /// <summary>Update budget alert</summary>
    [HttpPut("budgets/{id}")]
    [Authorize(Roles = "Admin,PlatformEngineer")]
    public async Task<IActionResult> UpdateBudgetAlert(Guid id, [FromBody] UpdateBudgetAlertRequest request)
    {
        var alert = await _context.BudgetAlerts.FindAsync(id);
        if (alert == null)
            return NotFound(new { message = $"Budget alert {id} not found" });

        alert.BudgetLimit = request.BudgetLimit;
        alert.AlertThresholdPercent = request.AlertThresholdPercent;
        alert.CurrentSpend = request.CurrentSpend;
        alert.IsActive = request.IsActive;
        alert.UpdatedAt = DateTime.UtcNow;

        if (request.CurrentSpend >= alert.BudgetLimit * (alert.AlertThresholdPercent / 100))
        {
            alert.IsTriggered = true;
            alert.TriggeredAt = DateTime.UtcNow;
            _logger.LogWarning("Budget alert triggered for team {Team}: {Spend}/{Limit}",
                alert.TeamName, request.CurrentSpend, alert.BudgetLimit);
        }

        await _context.SaveChangesAsync();
        return Ok(alert);
    }

    /// <summary>Get top expensive services</summary>
    [HttpGet("top-services")]
    public async Task<IActionResult> GetTopServices([FromQuery] int top = 10)
    {
        var serviceCosts = await _context.ServiceCosts
            .GroupBy(s => s.ServiceName)
            .Select(g => new
            {
                serviceName = g.Key,
                totalCost = g.Sum(s => s.TotalCost),
                avgCpuUsage = g.Average(s => s.CpuUsage),
                avgMemoryUsage = g.Average(s => s.MemoryUsage)
            })
            .OrderByDescending(x => x.totalCost)
            .Take(top)
            .ToListAsync();

        return Ok(serviceCosts);
    }
}

// Request DTOs
public record CreateCostReportRequest(
    string TeamName,
    string Namespace,
    string Period,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    decimal TotalCost,
    decimal CpuCost,
    decimal MemoryCost,
    decimal StorageCost,
    decimal NetworkCost,
    string Currency = "USD"
);

public record CreateBudgetAlertRequest(
    string TeamName,
    string Namespace,
    decimal BudgetLimit,
    decimal AlertThresholdPercent = 80,
    string Currency = "USD"
);

public record UpdateBudgetAlertRequest(
    decimal BudgetLimit,
    decimal AlertThresholdPercent,
    decimal CurrentSpend,
    bool IsActive
);
