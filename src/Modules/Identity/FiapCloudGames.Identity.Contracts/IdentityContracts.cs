namespace FiapCloudGames.Identity.Contracts;

public sealed record UserSummary(
    Guid Id,
    string Name,
    string Email,
    string Role,
    bool IsActive);

public interface IIdentityModule
{
    Task<UserSummary?> GetUserAsync(Guid userId, CancellationToken cancellationToken);
}

public sealed record UserDeactivatedIntegrationEvent(Guid UserId, DateTimeOffset OccurredAtUtc);
