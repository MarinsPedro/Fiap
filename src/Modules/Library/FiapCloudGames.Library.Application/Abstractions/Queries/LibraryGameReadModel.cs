namespace FiapCloudGames.Library.Application.Abstractions.Queries;

public sealed record LibraryGameReadModel(
    Guid Id,
    Guid GameId,
    decimal PricePaid,
    Guid? PromotionId,
    DateTimeOffset AcquiredAtUtc);
