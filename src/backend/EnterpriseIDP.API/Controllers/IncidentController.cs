using EnterpriseIDP.Application.Features.Incidents.Commands;
using EnterpriseIDP.Application.Features.Incidents.Queries;
using EnterpriseIDP.Application.Interfaces;
using EnterpriseIDP.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseIDP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncidentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuditService _auditService;
    private readonly ILogger<IncidentController> _logger;

    public IncidentController(
        IMediator mediator,
        IAuditService auditService,
        ILogger<IncidentController> logger)
    {
        _mediator = mediator;
        _auditService = auditService;
        _logger = logger;
    }

    /// <summary>Get all incidents with filtering</summary>
    [HttpGet]
    public async Task<IActionResult> GetIncidents(
        [FromQuery] IncidentStatus? status,
        [FromQuery] IncidentSeverity? severity,
        [FromQuery] string? affectedService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetIncidentsQuery(status, severity, affectedService, page, pageSize));
        return Ok(result);
    }

    /// <summary>Get incident by ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetIncident(Guid id)
    {
        var incident = await _mediator.Send(new GetIncidentByIdQuery(id));
        if (incident == null)
            return NotFound(new { message = $"Incident {id} not found" });
        return Ok(incident);
    }

    /// <summary>Get incident stats</summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _mediator.Send(new GetIncidentStatsQuery());
        return Ok(stats);
    }

    /// <summary>Create new incident</summary>
    [HttpPost]
    public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentRequest request)
    {
        var incident = await _mediator.Send(new CreateIncidentCommand(
            request.Title,
            request.Description,
            request.Severity,
            request.AffectedService,
            request.AssignedTo,
            request.Labels
        ));

        await _auditService.LogAsync("CREATE", "Incident", incident.Id.ToString(),
            newValues: $"Title={incident.Title}, Severity={incident.Severity}");

        return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incident);
    }

    /// <summary>Update incident</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncident(Guid id, [FromBody] UpdateIncidentRequest request)
    {
        var incident = await _mediator.Send(new UpdateIncidentCommand(
            id,
            request.Title,
            request.Description,
            request.Severity,
            request.Status,
            request.AssignedTo,
            request.RootCause,
            request.Resolution
        ));

        await _auditService.LogAsync("UPDATE", "Incident", id.ToString(),
            newValues: $"Status={incident.Status}");

        return Ok(incident);
    }

    /// <summary>Resolve incident</summary>
    [HttpPost("{id}/resolve")]
    public async Task<IActionResult> ResolveIncident(Guid id, [FromBody] ResolveIncidentRequest request)
    {
        var incident = await _mediator.Send(new ResolveIncidentCommand(
            id, request.Resolution, request.RootCause));

        await _auditService.LogAsync("RESOLVE", "Incident", id.ToString(),
            newValues: $"RootCause={request.RootCause}");

        return Ok(incident);
    }

    /// <summary>Add timeline event</summary>
    [HttpPost("{id}/timeline")]
    public async Task<IActionResult> AddTimelineEvent(Guid id, [FromBody] AddTimelineEventRequest request)
    {
        var timeline = await _mediator.Send(new AddTimelineEventCommand(
            id, request.Message, request.EventType));
        return Ok(timeline);
    }

    /// <summary>Get incident timeline</summary>
    [HttpGet("{id}/timeline")]
    public async Task<IActionResult> GetTimeline(Guid id)
    {
        var timeline = await _mediator.Send(new GetIncidentTimelineQuery(id));
        return Ok(timeline);
    }

    /// <summary>Create postmortem</summary>
    [HttpPost("{id}/postmortem")]
    public async Task<IActionResult> CreatePostmortem(Guid id, [FromBody] CreatePostmortemRequest request)
    {
        var postmortem = await _mediator.Send(new CreatePostmortemCommand(
            id,
            request.Summary,
            request.Impact,
            request.RootCause,
            request.Timeline,
            request.ActionItems,
            request.LessonsLearned
        ));

        await _auditService.LogAsync("CREATE", "Postmortem", postmortem.Id.ToString(),
            newValues: $"IncidentId={id}");

        return Ok(postmortem);
    }

    /// <summary>Get postmortem by incident</summary>
    [HttpGet("{id}/postmortem")]
    public async Task<IActionResult> GetPostmortem(Guid id)
    {
        var postmortem = await _mediator.Send(new GetPostmortemByIncidentQuery(id));
        if (postmortem == null)
            return NotFound(new { message = $"No postmortem found for incident {id}" });
        return Ok(postmortem);
    }
}

// Request DTOs
public record CreateIncidentRequest(
    string Title,
    string Description,
    IncidentSeverity Severity,
    string AffectedService,
    string AssignedTo,
    List<string> Labels
);

public record UpdateIncidentRequest(
    string Title,
    string Description,
    IncidentSeverity Severity,
    IncidentStatus Status,
    string AssignedTo,
    string? RootCause,
    string? Resolution
);

public record ResolveIncidentRequest(
    string Resolution,
    string RootCause
);

public record AddTimelineEventRequest(
    string Message,
    string EventType
);

public record CreatePostmortemRequest(
    string Summary,
    string Impact,
    string RootCause,
    string Timeline,
    string ActionItems,
    string LessonsLearned
);
