namespace FiapCloudGames.Identity.Application.Abstractions.Security;

public sealed record GeneratedToken(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc);
