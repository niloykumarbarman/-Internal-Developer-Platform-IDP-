using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.CICD;
using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.CICD.Commands.TriggerPipeline;

public class TriggerPipelineHandler : IRequestHandler<TriggerPipelineCommand, TriggerPipelineResult>
{
    private readonly IRepository<Pipeline> _pipelineRepo;
    private readonly IRepository<PipelineRun> _runRepo;
    private readonly IUnitOfWork _uow;

    public TriggerPipelineHandler(IRepository<Pipeline> pipelineRepo, IRepository<PipelineRun> runRepo, IUnitOfWork uow)
    {
        _pipelineRepo = pipelineRepo;
        _runRepo = runRepo;
        _uow = uow;
    }

    public async Task<TriggerPipelineResult> Handle(TriggerPipelineCommand cmd, CancellationToken ct)
    {
        var pipeline = await _pipelineRepo.GetByIdAsync(cmd.PipelineId, ct)
            ?? throw new NotFoundException(nameof(Pipeline), cmd.PipelineId);

        var runNumber = pipeline.Runs.Count + 1;

        var run = PipelineRun.Create(
            pipeline.Id,
            runNumber,
            null,
            null,
            cmd.TriggeredBy.ToString()
        );

        pipeline.Start(Guid.NewGuid().ToString(), cmd.TriggeredBy.ToString());

        await _runRepo.AddAsync(run, ct);
        await _pipelineRepo.UpdateAsync(pipeline, ct);
        await _uow.SaveChangesAsync(ct);

        return new TriggerPipelineResult(
            run.Id, run.PipelineId, cmd.Branch, run.Status, run.StartedAt ?? DateTime.UtcNow
        );
    }
}
