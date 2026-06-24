using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Application.Common.Extensions;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Auth;
using MediatR;

namespace EnterpriseIDP.Application.Features.Auth.Commands.CreateTeam;

public class CreateTeamHandler : IRequestHandler<CreateTeamCommand, CreateTeamResult>
{
    private readonly IRepository<Team> _teamRepo;
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _uow;

    public CreateTeamHandler(IRepository<Team> teamRepo, IRepository<User> userRepo, IUnitOfWork uow)
    {
        _teamRepo = teamRepo;
        _userRepo = userRepo;
        _uow = uow;
    }

    public async Task<CreateTeamResult> Handle(CreateTeamCommand cmd, CancellationToken ct)
    {
        var owner = await _userRepo.GetByIdAsync(cmd.OwnerId, ct)
            ?? throw new NotFoundException(nameof(User), cmd.OwnerId);

        var existing = await _teamRepo.FindAsync(t => t.Name == cmd.Name, ct);
        if (existing.Count > 0)
            throw new ConflictException($"Team '{cmd.Name}' already exists.");

        var slug = cmd.Name.ToLowerInvariant().Trim().Replace(" ", "-");

        var team = Team.Create(cmd.Name, slug, cmd.OwnerId, cmd.Description, owner.Id.ToString())
            .ThrowIfError();

        await _teamRepo.AddAsync(team, ct);
        await _uow.SaveChangesAsync(ct);

        return new CreateTeamResult(team.Id, team.Name, team.Description ?? string.Empty, team.OwnerId);
    }
}
