using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Promotions.Application.Features.Promotions;
using FiapCloudGames.Promotions.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Promotions.Application.Features.Promotions.GetPromotion;

public sealed class GetPromotionService(
    IPromotionRepository promotions,
    ILogger<GetPromotionService> logger)
{
    public async Task<PromotionResult> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "Consultando promoção {PromotionId}.",
            id);

        var promotion = await promotions.GetAsync(id, cancellationToken);

        if (promotion is null)
        {
            logger.LogDebug(
                "Promoção {PromotionId} não encontrada.",
                id);
            throw AppException.NotFound("Promoção não encontrada.");
        }

        return PromotionApplicationMappings.ToResult(promotion);
    }
}
