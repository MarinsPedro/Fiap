using FiapCloudGames.Identity.Application.Features.Users.UpdateUser;

namespace FiapCloudGames.Identity.Presentation.Features.Users.UpdateUser;

/// <summary>
/// Classe de mapeamento para a atualização de usuário via HTTP.
/// </summary>
internal static class UpdateUserHttpMapping
{
    public static UpdateUserInput ToInput(this UpdateUserRequest request) =>
        new(request.Name, request.Email);
}
