namespace FiapCloudGames.Library.Contracts;

public sealed record LibraryItemSnapshot(
    Guid Id,
    Guid GameId,
    string GameTitle,
    decimal PricePaid,
    Guid? PromotionId,
    DateTimeOffset AcquiredAtUtc);
