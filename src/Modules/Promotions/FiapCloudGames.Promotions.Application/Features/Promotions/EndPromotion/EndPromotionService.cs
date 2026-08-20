using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Promotions.Application.Features.Promotions.EndPromotion;

public sealed class EndPromotionService(
    IPromotionRepository promotions,
    IPromotionsUnitOfWork unitOfWork,
    TimeProvider clock,
    ILogger<EndPromotionService> logger)
{
    public async Task ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Iniciando encerramento da promoção {PromotionId}.",
            id);

        var promotion = await promotions.GetAsync(id, cancellationToken);
        if (promotion is null)
        {
            logger.LogWarning(
                "Não foi possível encerrar: promoção {PromotionId} não encontrada.",
                id);
            throw AppException.NotFound("Promoção não encontrada.");
        }

        promotion.End(clock.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Promoção {PromotionId} encerrada com sucesso.",
            promotion.Id);
    }
}
