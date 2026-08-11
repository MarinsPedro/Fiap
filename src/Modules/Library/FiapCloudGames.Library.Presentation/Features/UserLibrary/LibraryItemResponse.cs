namespace FiapCloudGames.Library.Presentation.Features.UserLibrary;

public sealed record LibraryItemResponse(
    Guid Id,
    Guid GameId,
    string GameTitle,
    decimal PricePaid,
    Guid? PromotionId,
    DateTimeOffset AcquiredAtUtc);
