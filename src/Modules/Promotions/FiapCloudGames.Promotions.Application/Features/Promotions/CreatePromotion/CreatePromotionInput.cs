namespace FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;

public sealed record CreatePromotionInput(
    string Name,
    decimal DiscountPercent,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    IReadOnlyCollection<Guid> GameIds);
