using FiapCloudGames.Identity.Application.Features.Users;
using FiapCloudGames.Identity.Domain.Repositories;

namespace FiapCloudGames.Identity.Application.Features.Users.GetUser;

public sealed class GetUserService(IUserRepository users)
{
    public async Task<UserResult?> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var user = await users.GetAsync(id, cancellationToken);

        return user is null
            ? null
            : IdentityApplicationMappings.ToResult(user);
    }
}
