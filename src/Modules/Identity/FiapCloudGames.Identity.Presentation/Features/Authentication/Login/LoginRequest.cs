using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Identity.Presentation.Features.Authentication.Login;

public sealed record LoginRequest(
    [Required, EmailAddress, StringLength(254)]
    string Email,

    [Required]
    string Password);
