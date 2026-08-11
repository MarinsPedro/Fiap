using FiapCloudGames.Identity.Application.Features.Users;

namespace FiapCloudGames.Identity.Application.Features.Authentication.Login;

public sealed record LoginResult(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    UserResult User);
