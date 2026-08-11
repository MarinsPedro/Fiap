using FiapCloudGames.Identity.Application.Features.Users.GetUser;
using FiapCloudGames.Identity.Contracts;

namespace FiapCloudGames.Identity.Application.Integrations;

internal sealed class IdentityModule(GetUserService getUserService)
    : IIdentityModule
{
    public async Task<UserSnapshot?> GetUserAsync(
        GetUserQuery query,
        CancellationToken cancellationToken)
    {
        var result = await getUserService.ExecuteAsync(
            query.UserId,
            cancellationToken);

        return result is null
            ? null
            : new UserSnapshot(result.Id, result.IsActive);
    }
}
