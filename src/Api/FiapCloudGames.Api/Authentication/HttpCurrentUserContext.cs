using System.Security.Claims;
using FiapCloudGames.Application.Common.Authentication;

namespace FiapCloudGames.Api.Authentication;

internal sealed class HttpCurrentUserContext(
    IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public Guid? UserId =>
        Guid.TryParse(
            httpContextAccessor.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var userId)
                ? userId
                : null;
}
