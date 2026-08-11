using FiapCloudGames.Promotions.Application.Features.Pricing.GetPromotionalPrice;
using FiapCloudGames.Promotions.Contracts;

namespace FiapCloudGames.Promotions.Application.Integrations;

internal sealed class PromotionsModule(GetPromotionalPriceService service)
    : IPromotionsModule
{
    public async Task<PriceQuoteSnapshot> GetPriceAsync(
        GetPriceQuoteQuery query,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            query.GameId,
            query.BasePrice,
            cancellationToken);

        return new PriceQuoteSnapshot(
            result.BasePrice,
            result.FinalPrice,
            result.DiscountPercent,
            result.PromotionId);
    }
}
