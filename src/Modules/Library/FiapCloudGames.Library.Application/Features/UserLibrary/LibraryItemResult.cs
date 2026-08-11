namespace FiapCloudGames.Library.Application.Features.UserLibrary;

public sealed record LibraryItemResult(
    Guid Id,
    Guid GameId,
    string GameTitle,
    decimal PricePaid,
    Guid? PromotionId,
    DateTimeOffset AcquiredAtUtc);
