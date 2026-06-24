using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Enums;
using ErrorOr;

namespace EnterpriseIDP.Domain.Entities.CICD;

public sealed class Pipeline : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid ServiceId { get; private set; }
    public string RepositoryUrl { get; private set; } = string.Empty;
    public string Branch { get; private set; } = string.Empty;
    public PipelineStatus Status { get; private set; }
    public string? WorkflowFile { get; private set; }
    public int RunNumber { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }
    public string? TriggeredBy { get; private set; }
    public string? GitHubRunId { get; private set; }
    public string? LogsUrl { get; private set; }

    private readonly List<PipelineRun> _runs = [];
    public IReadOnlyList<PipelineRun> Runs => _runs.AsReadOnly();

    private Pipeline() { }

    public static ErrorOr<Pipeline> Create(
        string name,
        Guid serviceId,
        string repositoryUrl,
        string branch,
        string? workflowFile,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Pipeline.Name", "Pipeline name cannot be empty.");

        return new Pipeline
        {
            Name = name.Trim(),
            ServiceId = serviceId,
            RepositoryUrl = repositoryUrl,
            Branch = branch,
            Status = PipelineStatus.Queued,
            WorkflowFile = workflowFile,
            CreatedBy = createdBy
        };
    }

    public void Start(string gitHubRunId, string triggeredBy)
    {
        Status = PipelineStatus.InProgress;
        GitHubRunId = gitHubRunId;
        TriggeredBy = triggeredBy;
        StartedAt = DateTime.UtcNow;
        RunNumber++;
    }

    public void Complete(bool success, string? logsUrl)
    {
        Status = success ? PipelineStatus.Success : PipelineStatus.Failed;
        FinishedAt = DateTime.UtcNow;
        LogsUrl = logsUrl;
    }

    public void Cancel(string cancelledBy)
    {
        Status = PipelineStatus.Cancelled;
        FinishedAt = DateTime.UtcNow;
        SetUpdated(cancelledBy);
    }

    public TimeSpan? Duration => StartedAt.HasValue && FinishedAt.HasValue
        ? FinishedAt - StartedAt
        : null;
}
