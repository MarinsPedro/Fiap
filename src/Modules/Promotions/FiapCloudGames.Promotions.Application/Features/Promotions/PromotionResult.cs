namespace FiapCloudGames.Promotions.Application.Features.Promotions;

public sealed record PromotionResult(
    Guid Id,
    string Name,
    decimal DiscountPercent,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    IReadOnlyCollection<Guid> GameIds);
