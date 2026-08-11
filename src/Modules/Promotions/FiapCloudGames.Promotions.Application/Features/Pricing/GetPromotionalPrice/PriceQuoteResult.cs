namespace FiapCloudGames.Promotions.Application.Features.Pricing.GetPromotionalPrice;

public sealed record PriceQuoteResult(
    decimal BasePrice,
    decimal FinalPrice,
    decimal DiscountPercent,
    Guid? PromotionId);
