using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.CICD;
using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.CICD.Queries.GetPipelines;

public class GetPipelinesHandler : IRequestHandler<GetPipelinesQuery, List<PipelineDto>>
{
    private readonly IRepository<Pipeline> _pipelineRepo;

    public GetPipelinesHandler(IRepository<Pipeline> pipelineRepo) => _pipelineRepo = pipelineRepo;

    public async Task<List<PipelineDto>> Handle(GetPipelinesQuery query, CancellationToken ct)
    {
        var all = await _pipelineRepo.GetAllAsync(ct);
        return all
            .Where(p => !query.ServiceId.HasValue || p.ServiceId == query.ServiceId)
            .Select(p => new PipelineDto(
                p.Id,
                p.Name,
                p.WorkflowFile ?? "CI",
                p.ServiceId,
                p.Status != PipelineStatus.Cancelled,
                p.Status,
                p.CreatedAt))
            .ToList();
    }
}
