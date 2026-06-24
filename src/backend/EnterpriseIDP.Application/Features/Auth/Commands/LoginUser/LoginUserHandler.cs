using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Application.Common.Interfaces;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Auth;
using EnterpriseIDP.Domain.ValueObjects;
using MediatR;

namespace EnterpriseIDP.Application.Features.Auth.Commands.LoginUser;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IRepository<User> _userRepo;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public LoginUserHandler(IRepository<User> userRepo, IPasswordHasher hasher, IJwtTokenGenerator jwt)
    {
        _userRepo = userRepo;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand cmd, CancellationToken ct)
    {
        var emailResult = Email.Create(cmd.Email);
        if (emailResult.IsError)
            throw new UnauthorizedException("Invalid email or password.");

        var users = await _userRepo.FindAsync(u => u.Email == emailResult.Value, ct);
        var user = users.FirstOrDefault()
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!_hasher.Verify(cmd.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        user.RecordLogin();

        var token = _jwt.GenerateToken(user.Id, user.Email.Value, user.Role.ToString());
        return new LoginUserResult(user.Id, user.FirstName, user.LastName, user.Email.Value, user.Role.ToString(), token);
    }
}
