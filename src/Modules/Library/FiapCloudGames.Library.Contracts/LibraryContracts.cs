namespace FiapCloudGames.Library.Contracts;

public sealed record LibraryItemSummary(
    Guid Id,
    Guid GameId,
    string GameTitle,
    decimal PricePaid,
    Guid? PromotionId,
    DateTimeOffset AcquiredAtUtc);

public sealed record UserLibrarySummary(Guid UserId, IReadOnlyCollection<LibraryItemSummary> Games);

public interface ILibraryModule
{
    Task<UserLibrarySummary> GetLibraryAsync(Guid userId, CancellationToken cancellationToken);
}

public sealed record GameAddedToLibraryIntegrationEvent(
    Guid UserId,
    Guid GameId,
    decimal PricePaid,
    DateTimeOffset OccurredAtUtc);
