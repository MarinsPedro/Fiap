using FiapCloudGames.Promotions.Application.Features.Promotions;
using FiapCloudGames.Promotions.Domain.Repositories;

namespace FiapCloudGames.Promotions.Application.Features.Promotions.GetPromotion;

public sealed class GetPromotionService(IPromotionRepository promotions)
{
    public async Task<PromotionResult?> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var promotion = await promotions.GetAsync(id, cancellationToken);

        return promotion is null
            ? null
            : PromotionApplicationMappings.ToResult(promotion);
    }
}
