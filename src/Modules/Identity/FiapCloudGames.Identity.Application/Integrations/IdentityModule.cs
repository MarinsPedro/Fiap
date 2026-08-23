using FiapCloudGames.Identity.Application.Features.Users.FindUser;
using FiapCloudGames.Identity.Contracts;

namespace FiapCloudGames.Identity.Application.Integrations;

internal sealed class IdentityModule(FindUserService findUser)
    : IIdentityModule
{
    public async Task<UserSnapshot?> GetUserAsync(
        GetUserQuery query,
        CancellationToken cancellationToken)
    {
        var result = await findUser.ExecuteAsync(
            query.UserId,
            cancellationToken);

        return result is null
            ? null
            : new UserSnapshot(result.Id, result.IsActive);
    }
}
