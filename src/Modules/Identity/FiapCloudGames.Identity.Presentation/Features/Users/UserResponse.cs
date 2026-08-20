namespace FiapCloudGames.Identity.Presentation.Features.Users;

/// <summary>
/// Objeto response para retorno dos dados do usuário da plataforma Fiap Cloud Game.
/// </summary>
/// <param name="Id">Identificador do usuário.</param>
/// <param name="Name">Nome do usuário.</param>
/// <param name="Email">Email do usuário.</param>
/// <param name="Role">Função do usuário.</param>
/// <param name="IsActive">Indica se o usuário está ativo.</param>
public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    string Role,
    bool IsActive);
