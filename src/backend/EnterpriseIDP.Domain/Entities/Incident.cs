namespace EnterpriseIDP.Domain.Entities;

public enum IncidentSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum IncidentStatus
{
    Open,
    Investigating,
    Identified,
    Monitoring,
    Resolved,
    Closed
}

public class Incident
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public string AffectedService { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public string? RootCause { get; set; }
    public string? Resolution { get; set; }
    public List<IncidentTimeline> Timeline { get; set; } = new();
    public List<string> Labels { get; set; } = new();
}

public class IncidentTimeline
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IncidentId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Incident? Incident { get; set; }
}

public class Postmortem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IncidentId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
    public string RootCause { get; set; } = string.Empty;
    public string Timeline { get; set; } = string.Empty;
    public string ActionItems { get; set; } = string.Empty;
    public string LessonsLearned { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Incident? Incident { get; set; }
}
