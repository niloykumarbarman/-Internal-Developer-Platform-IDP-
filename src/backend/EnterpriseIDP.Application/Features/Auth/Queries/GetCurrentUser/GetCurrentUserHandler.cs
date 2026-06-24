using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Auth;
using MediatR;

namespace EnterpriseIDP.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly IRepository<User> _userRepo;

    public GetCurrentUserHandler(IRepository<User> userRepo) => _userRepo = userRepo;

    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery query, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(query.UserId, ct)
            ?? throw new NotFoundException(nameof(User), query.UserId);

        return new CurrentUserDto(user.Id, user.FirstName, user.LastName, user.Email.Value, user.Role.ToString(), user.CreatedAt);
    }
}
