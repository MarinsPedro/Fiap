using FiapCloudGames.Identity.Application.Features.Users;

namespace FiapCloudGames.Identity.Presentation.Features.Users;

internal static class UserResponseMappings
{
    public static UserResponse ToResponse(this UserResult result) =>
        new(
            result.Id,
            result.Name,
            result.Email,
            result.Role,
            result.IsActive);
}
