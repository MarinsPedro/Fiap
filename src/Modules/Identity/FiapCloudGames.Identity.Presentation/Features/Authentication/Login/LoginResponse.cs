using FiapCloudGames.Identity.Presentation.Features.Users;

namespace FiapCloudGames.Identity.Presentation.Features.Authentication.Login;

public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    UserResponse User);
