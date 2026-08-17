using FiapCloudGames.Identity.Application.Features.Users.UpdateUser;

namespace FiapCloudGames.Identity.Presentation.Features.Users.UpdateUser;

internal static class UpdateUserHttpMapping
{
    public static UpdateUserInput ToInput(this UpdateUserRequest request) =>
        new(request.Name, request.Email);
}
