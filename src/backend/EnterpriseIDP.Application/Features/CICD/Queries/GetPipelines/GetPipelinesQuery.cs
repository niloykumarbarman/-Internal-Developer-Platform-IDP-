using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.CICD.Queries.GetPipelines;

public record GetPipelinesQuery(Guid? ServiceId = null) : IRequest<List<PipelineDto>>;

public record PipelineDto(
    Guid Id, string Name, string Type,
    Guid ServiceId, bool IsActive,
    PipelineStatus? LastRunStatus, DateTime CreatedAt
);
