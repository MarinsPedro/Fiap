using FiapCloudGames.Promotions.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Promotions.Application.Features.Pricing.GetPromotionalPrice;

public sealed class GetPromotionalPriceService(
    IPromotionRepository promotions,
    TimeProvider clock,
    ILogger<GetPromotionalPriceService> logger)
{
    public async Task<PriceQuoteResult> ExecuteAsync(
        Guid gameId,
        decimal basePrice,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "Consultando promoção ativa para o jogo {GameId}.",
            gameId);

        var promotion = await promotions.GetActiveForGameAsync(
            gameId,
            clock.GetUtcNow(),
            cancellationToken);

        if (promotion is null)
        {
            logger.LogDebug(
                "Nenhuma promoção ativa encontrada para o jogo {GameId}.",
                gameId);
            return new PriceQuoteResult(basePrice, basePrice, 0, null);
        }

        logger.LogInformation(
            "Promoção {PromotionId} aplicada ao jogo {GameId} com desconto de {DiscountPercent} por cento.",
            promotion.Id,
            gameId,
            promotion.DiscountPercent.Value);

        return new PriceQuoteResult(
            basePrice,
            promotion.ApplyTo(basePrice),
            promotion.DiscountPercent.Value,
            promotion.Id);
    }
}
