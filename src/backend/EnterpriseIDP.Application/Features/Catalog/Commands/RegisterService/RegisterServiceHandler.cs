using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Application.Common.Extensions;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Auth;
using EnterpriseIDP.Domain.Entities.Catalog;
using EnterpriseIDP.Domain.ValueObjects;
using MediatR;

namespace EnterpriseIDP.Application.Features.Catalog.Commands.RegisterService;

public class RegisterServiceHandler : IRequestHandler<RegisterServiceCommand, RegisterServiceResult>
{
    private readonly IRepository<Service> _serviceRepo;
    private readonly IRepository<Team> _teamRepo;
    private readonly IUnitOfWork _uow;

    public RegisterServiceHandler(IRepository<Service> serviceRepo, IRepository<Team> teamRepo, IUnitOfWork uow)
    {
        _serviceRepo = serviceRepo;
        _teamRepo = teamRepo;
        _uow = uow;
    }

    public async Task<RegisterServiceResult> Handle(RegisterServiceCommand cmd, CancellationToken ct)
    {
        var team = await _teamRepo.GetByIdAsync(cmd.TeamId, ct)
            ?? throw new NotFoundException(nameof(Team), cmd.TeamId);

        var slug = ServiceSlug.Create(cmd.Name).ThrowIfError();

        var existing = await _serviceRepo.FindAsync(s => s.Slug == slug, ct);
        if (existing.Count > 0)
            throw new ConflictException($"Service '{cmd.Name}' already exists.");

        var service = Service.Create(
            cmd.Name, slug.Value, cmd.Description, cmd.Type,
            cmd.TeamId, team.OwnerId, cmd.RepositoryUrl, cmd.Owner
        ).ThrowIfError();

        foreach (var tag in cmd.Tags)
            service.AddTag("tag", tag, cmd.Owner);

        await _serviceRepo.AddAsync(service, ct);
        await _uow.SaveChangesAsync(ct);

        return new RegisterServiceResult(service.Id, service.Name, service.Slug.Value, service.Type, cmd.Owner, service.Status, service.CreatedAt);
    }
}
