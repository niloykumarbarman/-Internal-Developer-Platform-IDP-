using EnterpriseIDP.Application.Features.Incidents.Queries;
using EnterpriseIDP.Application.Interfaces;
using EnterpriseIDP.Domain.Entities;
using EnterpriseIDP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseIDP.Infrastructure.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly ApplicationDbContext _context;

    public IncidentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Incidents
            .Include(i => i.Timeline)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<GetIncidentsResult> GetAllAsync(GetIncidentsQuery query, CancellationToken cancellationToken = default)
    {
        var q = _context.Incidents
            .Include(i => i.Timeline)
            .AsQueryable();

        if (query.Status.HasValue)
            q = q.Where(i => i.Status == query.Status.Value);

        if (query.Severity.HasValue)
            q = q.Where(i => i.Severity == query.Severity.Value);

        if (!string.IsNullOrEmpty(query.AffectedService))
            q = q.Where(i => i.AffectedService == query.AffectedService);

        var totalCount = await q.CountAsync(cancellationToken);

        var items = await q
            .OrderByDescending(i => i.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new GetIncidentsResult
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<Incident> AddAsync(Incident incident, CancellationToken cancellationToken = default)
    {
        _context.Incidents.Add(incident);
        await _context.SaveChangesAsync(cancellationToken);
        return incident;
    }

    public async Task<Incident> UpdateAsync(Incident incident, CancellationToken cancellationToken = default)
    {
        _context.Incidents.Update(incident);
        await _context.SaveChangesAsync(cancellationToken);
        return incident;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var incident = await _context.Incidents.FindAsync(new object[] { id }, cancellationToken);
        if (incident == null) return false;

        _context.Incidents.Remove(incident);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IncidentTimeline> AddTimelineEventAsync(IncidentTimeline timeline, CancellationToken cancellationToken = default)
    {
        _context.IncidentTimelines.Add(timeline);
        await _context.SaveChangesAsync(cancellationToken);
        return timeline;
    }

    public async Task<List<IncidentTimeline>> GetTimelineAsync(Guid incidentId, CancellationToken cancellationToken = default)
    {
        return await _context.IncidentTimelines
            .Where(t => t.IncidentId == incidentId)
            .OrderBy(t => t.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<Postmortem> AddPostmortemAsync(Postmortem postmortem, CancellationToken cancellationToken = default)
    {
        _context.Postmortems.Add(postmortem);
        await _context.SaveChangesAsync(cancellationToken);
        return postmortem;
    }

    public async Task<Postmortem?> GetPostmortemByIncidentAsync(Guid incidentId, CancellationToken cancellationToken = default)
    {
        return await _context.Postmortems
            .FirstOrDefaultAsync(p => p.IncidentId == incidentId, cancellationToken);
    }

    public async Task<IncidentStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var incidents = await _context.Incidents.ToListAsync(cancellationToken);

        var resolved = incidents.Where(i => i.ResolvedAt.HasValue).ToList();
        var avgMttr = resolved.Any()
            ? resolved.Average(i => (i.ResolvedAt!.Value - i.CreatedAt).TotalMinutes)
            : 0;

        var byService = incidents
            .GroupBy(i => i.AffectedService)
            .Select(g => new IncidentByService { Service = g.Key, Count = g.Count() })
            .ToList();

        return new IncidentStats
        {
            TotalOpen = incidents.Count(i => i.Status == IncidentStatus.Open),
            TotalInvestigating = incidents.Count(i => i.Status == IncidentStatus.Investigating),
            TotalResolved = incidents.Count(i => i.Status == IncidentStatus.Resolved),
            CriticalCount = incidents.Count(i => i.Severity == IncidentSeverity.Critical),
            HighCount = incidents.Count(i => i.Severity == IncidentSeverity.High),
            AverageMTTR = Math.Round(avgMttr, 2),
            ByService = byService
        };
    }
}
