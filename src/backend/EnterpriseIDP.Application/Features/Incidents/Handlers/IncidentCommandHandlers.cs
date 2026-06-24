using EnterpriseIDP.Application.Common.Interfaces;
using EnterpriseIDP.Application.Features.Incidents.Commands;
using EnterpriseIDP.Application.Features.Incidents.Queries;
using EnterpriseIDP.Application.Interfaces;
using EnterpriseIDP.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EnterpriseIDP.Application.Features.Incidents.Handlers;

public class CreateIncidentHandler : IRequestHandler<CreateIncidentCommand, Incident>
{
    private readonly IIncidentRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateIncidentHandler> _logger;

    public CreateIncidentHandler(
        IIncidentRepository repository,
        ICurrentUserService currentUser,
        ILogger<CreateIncidentHandler> logger)
    {
        _repository = repository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Incident> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = new Incident
        {
            Title = request.Title,
            Description = request.Description,
            Severity = request.Severity,
            AffectedService = request.AffectedService,
            AssignedTo = request.AssignedTo,
            Labels = request.Labels,
            CreatedBy = _currentUser.UserId ?? "system",
            Status = IncidentStatus.Open
        };

        await _repository.AddAsync(incident, cancellationToken);

        var timeline = new IncidentTimeline
        {
            IncidentId = incident.Id,
            Message = $"Incident created: {incident.Title}",
            Author = incident.CreatedBy,
            EventType = "created"
        };
        await _repository.AddTimelineEventAsync(timeline, cancellationToken);

        _logger.LogInformation("Incident created: {Id} - {Title}", incident.Id, incident.Title);
        return incident;
    }
}

public class UpdateIncidentHandler : IRequestHandler<UpdateIncidentCommand, Incident>
{
    private readonly IIncidentRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UpdateIncidentHandler(IIncidentRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Incident> Handle(UpdateIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Incident {request.Id} not found");

        var oldStatus = incident.Status;

        incident.Title = request.Title;
        incident.Description = request.Description;
        incident.Severity = request.Severity;
        incident.Status = request.Status;
        incident.AssignedTo = request.AssignedTo;
        incident.RootCause = request.RootCause;
        incident.Resolution = request.Resolution;
        incident.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(incident, cancellationToken);

        if (oldStatus != request.Status)
        {
            var timeline = new IncidentTimeline
            {
                IncidentId = incident.Id,
                Message = $"Status changed from {oldStatus} to {request.Status}",
                Author = _currentUser.UserId ?? "system",
                EventType = "status_change"
            };
            await _repository.AddTimelineEventAsync(timeline, cancellationToken);
        }

        return incident;
    }
}

public class ResolveIncidentHandler : IRequestHandler<ResolveIncidentCommand, Incident>
{
    private readonly IIncidentRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public ResolveIncidentHandler(IIncidentRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Incident> Handle(ResolveIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Incident {request.Id} not found");

        incident.Status = IncidentStatus.Resolved;
        incident.Resolution = request.Resolution;
        incident.RootCause = request.RootCause;
        incident.ResolvedAt = DateTime.UtcNow;
        incident.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(incident, cancellationToken);

        var timeline = new IncidentTimeline
        {
            IncidentId = incident.Id,
            Message = $"Incident resolved. Root cause: {request.RootCause}",
            Author = _currentUser.UserId ?? "system",
            EventType = "resolved"
        };
        await _repository.AddTimelineEventAsync(timeline, cancellationToken);

        return incident;
    }
}

public class AddTimelineEventHandler : IRequestHandler<AddTimelineEventCommand, IncidentTimeline>
{
    private readonly IIncidentRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public AddTimelineEventHandler(IIncidentRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<IncidentTimeline> Handle(AddTimelineEventCommand request, CancellationToken cancellationToken)
    {
        var timeline = new IncidentTimeline
        {
            IncidentId = request.IncidentId,
            Message = request.Message,
            Author = _currentUser.UserId ?? "system",
            EventType = request.EventType
        };

        await _repository.AddTimelineEventAsync(timeline, cancellationToken);
        return timeline;
    }
}

public class CreatePostmortemHandler : IRequestHandler<CreatePostmortemCommand, Postmortem>
{
    private readonly IIncidentRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreatePostmortemHandler(IIncidentRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Postmortem> Handle(CreatePostmortemCommand request, CancellationToken cancellationToken)
    {
        var postmortem = new Postmortem
        {
            IncidentId = request.IncidentId,
            Summary = request.Summary,
            Impact = request.Impact,
            RootCause = request.RootCause,
            Timeline = request.Timeline,
            ActionItems = request.ActionItems,
            LessonsLearned = request.LessonsLearned,
            CreatedBy = _currentUser.UserId ?? "system"
        };

        await _repository.AddPostmortemAsync(postmortem, cancellationToken);
        return postmortem;
    }
}

public class GetIncidentsHandler : IRequestHandler<GetIncidentsQuery, GetIncidentsResult>
{
    private readonly IIncidentRepository _repository;

    public GetIncidentsHandler(IIncidentRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetIncidentsResult> Handle(GetIncidentsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request, cancellationToken);
    }
}

public class GetIncidentByIdHandler : IRequestHandler<GetIncidentByIdQuery, Incident?>
{
    private readonly IIncidentRepository _repository;

    public GetIncidentByIdHandler(IIncidentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Incident?> Handle(GetIncidentByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id, cancellationToken);
    }
}

public class GetIncidentStatsHandler : IRequestHandler<GetIncidentStatsQuery, IncidentStats>
{
    private readonly IIncidentRepository _repository;

    public GetIncidentStatsHandler(IIncidentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IncidentStats> Handle(GetIncidentStatsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetStatsAsync(cancellationToken);
    }
}
