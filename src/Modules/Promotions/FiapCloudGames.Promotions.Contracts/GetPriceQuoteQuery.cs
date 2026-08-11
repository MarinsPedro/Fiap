namespace FiapCloudGames.Promotions.Contracts;

public sealed record GetPriceQuoteQuery(
    Guid GameId,
    decimal BasePrice);
