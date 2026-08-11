using System.Security.Claims;

namespace FiapCloudGames.Presentation.Common.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetUserId(
        this ClaimsPrincipal principal,
        out Guid userId)
    {
        ArgumentNullException.ThrowIfNull(principal);

        return Guid.TryParse(
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out userId);
    }
}
