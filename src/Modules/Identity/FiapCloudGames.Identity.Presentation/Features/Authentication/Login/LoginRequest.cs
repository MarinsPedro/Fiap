using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Identity.Presentation.Features.Authentication.Login;

/// <summary>
/// Objeto request para envio com os dados para autenticação do usuário na plataforma Fiap Cloud Game.
/// </summary>
/// <param name="Email">O endereço de e-mail do usuário.</param>
/// <param name="Password">A senha do usuário, que deve obedecer às políticas de segurança da plataforma</param>
public sealed record LoginRequest(
    [Required, EmailAddress, StringLength(254)]
    string Email,

    [Required]
    string Password);
