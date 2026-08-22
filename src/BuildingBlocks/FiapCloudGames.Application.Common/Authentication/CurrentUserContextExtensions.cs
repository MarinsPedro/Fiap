using FiapCloudGames.Application.Common.Exceptions;

namespace FiapCloudGames.Application.Common.Authentication;

public static class CurrentUserContextExtensions
{
    public static Guid GetRequiredUserId(
        this ICurrentUserContext currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        return currentUser.UserId ??
            throw AppException.Authentication(
                "O identificador do usuário autenticado é inválido.");
    }
}
