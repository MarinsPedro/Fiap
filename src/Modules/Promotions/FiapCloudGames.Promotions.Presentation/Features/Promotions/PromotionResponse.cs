namespace FiapCloudGames.Promotions.Presentation.Features.Promotions;

public sealed record PromotionResponse(
    Guid Id,
    string Name,
    decimal DiscountPercent,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    IReadOnlyCollection<Guid> GameIds);
