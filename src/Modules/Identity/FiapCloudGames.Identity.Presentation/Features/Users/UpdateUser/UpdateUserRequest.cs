using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Identity.Presentation.Features.Users.UpdateUser;

/// <summary>
/// Request para atualização de usuário.
/// </summary>
/// <param name="Name">Nome do usuário.</param>
/// <param name="Email">Email do usuário.</param>
public sealed record UpdateUserRequest(
    [Required, StringLength(120, MinimumLength = 2)]
    string Name,

    [Required, EmailAddress, StringLength(254)]
    string Email);

