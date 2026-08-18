using FiapCloudGames.Identity.Application.Features.Authentication.Login;
using FiapCloudGames.Identity.Presentation.Features.Users;

namespace FiapCloudGames.Identity.Presentation.Features.Authentication.Login;

/// <summary>
/// Mapeamentos de conversão entre os objetos de request/response e os objetos de input/result do serviço de autenticação.
/// </summary>
internal static class LoginHttpMappings
{
    public static LoginInput ToInput(this LoginRequest request) =>
        new(request.Email, request.Password);

    public static LoginResponse ToResponse(this LoginResult result) =>
        new(
            result.AccessToken,
            result.ExpiresAtUtc,
            result.User.ToResponse());
}
