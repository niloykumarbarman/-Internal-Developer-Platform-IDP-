using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.CICD.Commands.CreatePipeline;

public record CreatePipelineCommand(
    string Name,
    Guid ServiceId,
    string RepositoryUrl,
    string Branch = "main",
    string? WorkflowFile = null
) : IRequest<CreatePipelineResult>;

public record CreatePipelineResult(
    Guid Id,
    string Name,
    Guid ServiceId,
    string RepositoryUrl,
    string Branch,
    PipelineStatus Status,
    DateTime CreatedAt
);
