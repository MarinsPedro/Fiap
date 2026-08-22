using FiapCloudGames.Application.Common.Authentication;
using FiapCloudGames.Identity.Application.Features.Users.GetUser;

namespace FiapCloudGames.Identity.Application.Features.Users.GetCurrentUser;

public sealed class GetCurrentUserService(
    ICurrentUserContext currentUser,
    GetUserService getUser)
{
    public Task<UserResult> ExecuteAsync(
        CancellationToken cancellationToken) =>
        getUser.ExecuteAsync(
            currentUser.GetRequiredUserId(),
            cancellationToken);
}
