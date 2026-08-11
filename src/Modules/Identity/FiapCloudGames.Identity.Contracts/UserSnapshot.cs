namespace FiapCloudGames.Identity.Contracts;

public sealed record UserSnapshot(
    Guid Id,
    bool IsActive);
