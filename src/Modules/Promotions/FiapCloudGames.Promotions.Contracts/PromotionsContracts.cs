namespace FiapCloudGames.Promotions.Contracts;

public sealed record PromotionSummary(
    Guid Id,
    string Name,
    decimal DiscountPercent,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    IReadOnlyCollection<Guid> GameIds);

public sealed record PriceQuote(
    decimal BasePrice,
    decimal FinalPrice,
    decimal DiscountPercent,
    Guid? PromotionId);

public interface IPromotionsModule
{
    Task<PriceQuote> GetPriceAsync(Guid gameId, decimal basePrice, CancellationToken cancellationToken);
}

public sealed record PromotionStartedIntegrationEvent(
    Guid PromotionId,
    IReadOnlyCollection<Guid> GameIds,
    DateTimeOffset OccurredAtUtc);
