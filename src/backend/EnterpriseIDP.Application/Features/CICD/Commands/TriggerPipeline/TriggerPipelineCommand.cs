using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.CICD.Commands.TriggerPipeline;

public record TriggerPipelineCommand(
    Guid PipelineId,
    string Branch,
    Guid TriggeredBy,
    Dictionary<string, string>? Parameters = null
) : IRequest<TriggerPipelineResult>;

public record TriggerPipelineResult(
    Guid RunId, Guid PipelineId, string Branch,
    PipelineStatus Status, DateTime StartedAt
);
