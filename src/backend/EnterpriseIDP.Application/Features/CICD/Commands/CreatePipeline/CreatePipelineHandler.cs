using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Application.Common.Extensions;
using EnterpriseIDP.Application.Common.Interfaces;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Catalog;
using EnterpriseIDP.Domain.Entities.CICD;
using MediatR;

namespace EnterpriseIDP.Application.Features.CICD.Commands.CreatePipeline;

public class CreatePipelineHandler : IRequestHandler<CreatePipelineCommand, CreatePipelineResult>
{
    private readonly IRepository<Pipeline> _pipelineRepo;
    private readonly IRepository<Service> _serviceRepo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreatePipelineHandler(IRepository<Pipeline> pipelineRepo, IRepository<Service> serviceRepo,
        IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _pipelineRepo = pipelineRepo;
        _serviceRepo = serviceRepo;
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<CreatePipelineResult> Handle(CreatePipelineCommand cmd, CancellationToken ct)
    {
        _ = await _serviceRepo.GetByIdAsync(cmd.ServiceId, ct)
            ?? throw new NotFoundException(nameof(Service), cmd.ServiceId);

        if (await _pipelineRepo.ExistsAsync(p => p.ServiceId == cmd.ServiceId && p.Name == cmd.Name, ct))
            throw new ConflictException($"Pipeline '{cmd.Name}' already exists for this service.");

        var pipeline = Pipeline.Create(
            cmd.Name,
            cmd.ServiceId,
            cmd.RepositoryUrl,
            cmd.Branch,
            cmd.WorkflowFile,
            _currentUser.UserId?.ToString() ?? "system"
        ).ThrowIfError();

        await _pipelineRepo.AddAsync(pipeline, ct);
        await _uow.SaveChangesAsync(ct);

        return new CreatePipelineResult(
            pipeline.Id, pipeline.Name, pipeline.ServiceId,
            pipeline.RepositoryUrl, pipeline.Branch, pipeline.Status, pipeline.CreatedAt
        );
    }
}
