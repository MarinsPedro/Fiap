using FiapCloudGames.Application.Common.Authentication;
using FiapCloudGames.Library.Application.Features.UserLibrary.GetLibrary;

namespace FiapCloudGames.Library.Application.Features.UserLibrary.GetCurrentLibrary;

public sealed class GetCurrentLibraryService(
    ICurrentUserContext currentUser,
    GetLibraryService getLibrary)
{
    public Task<UserLibraryResult> ExecuteAsync(
        CancellationToken cancellationToken) =>
        getLibrary.ExecuteAsync(
            currentUser.GetRequiredUserId(),
            cancellationToken);
}
