using EnterpriseIDP.Application.Features.Incidents.Queries;
using EnterpriseIDP.Domain.Entities;

namespace EnterpriseIDP.Application.Interfaces;

public interface IIncidentRepository
{
    Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GetIncidentsResult> GetAllAsync(GetIncidentsQuery query, CancellationToken cancellationToken = default);
    Task<Incident> AddAsync(Incident incident, CancellationToken cancellationToken = default);
    Task<Incident> UpdateAsync(Incident incident, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IncidentTimeline> AddTimelineEventAsync(IncidentTimeline timeline, CancellationToken cancellationToken = default);
    Task<List<IncidentTimeline>> GetTimelineAsync(Guid incidentId, CancellationToken cancellationToken = default);
    Task<Postmortem> AddPostmortemAsync(Postmortem postmortem, CancellationToken cancellationToken = default);
    Task<Postmortem?> GetPostmortemByIncidentAsync(Guid incidentId, CancellationToken cancellationToken = default);
    Task<IncidentStats> GetStatsAsync(CancellationToken cancellationToken = default);
}
