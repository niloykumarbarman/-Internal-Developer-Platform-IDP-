using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Enums;

namespace EnterpriseIDP.Domain.Entities.CICD;

public sealed class PipelineRun : BaseEntity
{
    public Guid PipelineId { get; private set; }
    public int RunNumber { get; private set; }
    public PipelineStatus Status { get; private set; }
    public string? GitHubRunId { get; private set; }
    public string? CommitSha { get; private set; }
    public string? CommitMessage { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }
    public string? TriggeredBy { get; private set; }
    public string? LogsUrl { get; private set; }

    public Pipeline Pipeline { get; private set; } = null!;

    private PipelineRun() { }

    public static PipelineRun Create(Guid pipelineId, int runNumber, string? commitSha, string? commitMessage, string triggeredBy)
    {
        return new PipelineRun
        {
            PipelineId = pipelineId,
            RunNumber = runNumber,
            Status = PipelineStatus.InProgress,
            CommitSha = commitSha,
            CommitMessage = commitMessage,
            StartedAt = DateTime.UtcNow,
            TriggeredBy = triggeredBy,
            CreatedBy = triggeredBy
        };
    }
}
