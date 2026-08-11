using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Repositories;

namespace FiapCloudGames.Promotions.Application.Features.Promotions.EndPromotion;

public sealed class EndPromotionService(
    IPromotionRepository promotions,
    IPromotionsUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var promotion = await promotions.GetAsync(id, cancellationToken)
            ?? throw AppException.NotFound("Promoção não encontrada.");
        promotion.End(clock.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
