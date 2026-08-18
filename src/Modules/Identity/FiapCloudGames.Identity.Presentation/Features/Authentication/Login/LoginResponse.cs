using FiapCloudGames.Identity.Presentation.Features.Users;

namespace FiapCloudGames.Identity.Presentation.Features.Authentication.Login;

/// <summary>
/// Objeto response para retorno dos dados de autenticação do usuário na plataforma Fiap Cloud Game.
/// </summary>
/// <param name="AccessToken">O token de acesso gerado para autenticação do usuário.</param>
/// <param name="ExpiresAtUtc">A data e hora de expiração do token de acesso em UTC.</param>
/// <param name="User">Os dados do usuário autenticado.</param>
public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    UserResponse User);
