namespace FiapCloudGames.Promotions.Contracts;

public interface IPromotionsModule
{
    Task<PriceQuoteSnapshot> GetPriceAsync(
        GetPriceQuoteQuery query,
        CancellationToken cancellationToken);
}
