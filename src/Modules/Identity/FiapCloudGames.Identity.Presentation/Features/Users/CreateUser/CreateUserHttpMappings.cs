using FiapCloudGames.Identity.Application.Features.Users.CreateUser;

namespace FiapCloudGames.Identity.Presentation.Features.Users.CreateUser;

/// <summary>
/// Classe de mapeamento de request para input do serviço de criação de usuário.
/// </summary>
internal static class CreateUserHttpMappings
{
    public static CreateUserInput ToInput(this CreateUserRequest request) =>
        new(request.Name, request.Email, request.Password);
}
