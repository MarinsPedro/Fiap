using FiapCloudGames.Promotions.Domain.Repositories;

namespace FiapCloudGames.Promotions.Application.Features.Pricing.GetPromotionalPrice;

public sealed class GetPromotionalPriceService(
    IPromotionRepository promotions,
    TimeProvider clock)
{
    public async Task<PriceQuoteResult> ExecuteAsync(
        Guid gameId,
        decimal basePrice,
        CancellationToken cancellationToken)
    {
        var promotion = await promotions.GetActiveForGameAsync(
            gameId,
            clock.GetUtcNow(),
            cancellationToken);

        return promotion is null
            ? new PriceQuoteResult(basePrice, basePrice, 0, null)
            : new PriceQuoteResult(
                basePrice,
                promotion.ApplyTo(basePrice),
                promotion.DiscountPercent.Value,
                promotion.Id);
    }
}
