using EnterpriseIDP.Domain.Entities;
using MediatR;

namespace EnterpriseIDP.Application.Features.Incidents.Commands;

public record CreateIncidentCommand(
    string Title,
    string Description,
    IncidentSeverity Severity,
    string AffectedService,
    string AssignedTo,
    List<string> Labels
) : IRequest<Incident>;

public record UpdateIncidentCommand(
    Guid Id,
    string Title,
    string Description,
    IncidentSeverity Severity,
    IncidentStatus Status,
    string AssignedTo,
    string? RootCause,
    string? Resolution
) : IRequest<Incident>;

public record ResolveIncidentCommand(
    Guid Id,
    string Resolution,
    string RootCause
) : IRequest<Incident>;

public record AddTimelineEventCommand(
    Guid IncidentId,
    string Message,
    string EventType
) : IRequest<IncidentTimeline>;

public record CreatePostmortemCommand(
    Guid IncidentId,
    string Summary,
    string Impact,
    string RootCause,
    string Timeline,
    string ActionItems,
    string LessonsLearned
) : IRequest<Postmortem>;
