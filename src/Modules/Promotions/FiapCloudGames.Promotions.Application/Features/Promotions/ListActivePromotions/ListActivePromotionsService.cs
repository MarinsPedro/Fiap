using FiapCloudGames.Promotions.Application.Features.Promotions;
using FiapCloudGames.Promotions.Domain.Repositories;

namespace FiapCloudGames.Promotions.Application.Features.Promotions.ListActivePromotions;

public sealed class ListActivePromotionsService(
    IPromotionRepository promotions,
    TimeProvider clock)
{
    public async Task<IReadOnlyList<PromotionResult>> ExecuteAsync(
        CancellationToken cancellationToken) =>
        (await promotions.ListActiveAsync(
            clock.GetUtcNow(),
            cancellationToken))
            .Select(PromotionApplicationMappings.ToResult)
            .ToArray();
}
