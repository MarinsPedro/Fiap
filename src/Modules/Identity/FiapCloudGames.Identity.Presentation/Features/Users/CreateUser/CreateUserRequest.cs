using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Identity.Presentation.Features.Users.CreateUser;

/// <summary>
/// Request para criação de usuário no sistema.
/// </summary>
/// <param name="Name">Nome do usuário.</param>
/// <param name="Email">Email do usuário.</param>
/// <param name="Password">Senha do usuário.</param>
public sealed record CreateUserRequest(
    [Required, StringLength(120, MinimumLength = 2)]
    string Name,

    [Required, EmailAddress, StringLength(254)]
    string Email,

    [Required, MinLength(8)]
    string Password);
