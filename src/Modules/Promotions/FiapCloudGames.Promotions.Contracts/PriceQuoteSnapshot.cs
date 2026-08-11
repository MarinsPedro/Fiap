namespace FiapCloudGames.Promotions.Contracts;

public sealed record PriceQuoteSnapshot(
    decimal BasePrice,
    decimal FinalPrice,
    decimal DiscountPercent,
    Guid? PromotionId);
