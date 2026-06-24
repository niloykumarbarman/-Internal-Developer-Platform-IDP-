using EnterpriseIDP.Domain.Entities;
using MediatR;

namespace EnterpriseIDP.Application.Features.Incidents.Queries;

public record GetIncidentsQuery(
    IncidentStatus? Status = null,
    IncidentSeverity? Severity = null,
    string? AffectedService = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<GetIncidentsResult>;

public record GetIncidentByIdQuery(Guid Id) : IRequest<Incident?>;

public record GetIncidentTimelineQuery(Guid IncidentId) : IRequest<List<IncidentTimeline>>;

public record GetPostmortemByIncidentQuery(Guid IncidentId) : IRequest<Postmortem?>;

public record GetIncidentStatsQuery : IRequest<IncidentStats>;

public class GetIncidentsResult
{
    public List<Incident> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class IncidentStats
{
    public int TotalOpen { get; set; }
    public int TotalInvestigating { get; set; }
    public int TotalResolved { get; set; }
    public int CriticalCount { get; set; }
    public int HighCount { get; set; }
    public double AverageMTTR { get; set; }
    public List<IncidentByService> ByService { get; set; } = new();
}

public class IncidentByService
{
    public string Service { get; set; } = string.Empty;
    public int Count { get; set; }
}
