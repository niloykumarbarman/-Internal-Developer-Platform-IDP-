using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Application.Common.Extensions;
using EnterpriseIDP.Application.Common.Interfaces;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Auth;
using EnterpriseIDP.Domain.Enums;
using EnterpriseIDP.Domain.ValueObjects;
using MediatR;

namespace EnterpriseIDP.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public RegisterUserHandler(IRepository<User> userRepo, IUnitOfWork uow, IPasswordHasher hasher, IJwtTokenGenerator jwt)
    {
        _userRepo = userRepo;
        _uow = uow;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        var emailResult = Email.Create(cmd.Email);
        if (emailResult.IsError)
            throw new ValidationException(emailResult.Errors.Select(e =>
                new FluentValidation.Results.ValidationFailure(e.Code, e.Description)));

        var existing = await _userRepo.FindAsync(u => u.Email == emailResult.Value, ct);
        if (existing.Count > 0)
            throw new ConflictException($"Email '{cmd.Email}' is already registered.");

        var role = Enum.Parse<UserRole>(cmd.Role);
        var user = User.Create(
            cmd.FirstName,
            cmd.LastName,
            cmd.Email,
            _hasher.Hash(cmd.Password),
            role,
            "system"
        ).ThrowIfError();

        await _userRepo.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var token = _jwt.GenerateToken(user.Id, user.Email.Value, user.Role.ToString());
        return new RegisterUserResult(user.Id, user.FirstName, user.LastName, user.Email.Value, user.Role.ToString(), token);
    }
}
