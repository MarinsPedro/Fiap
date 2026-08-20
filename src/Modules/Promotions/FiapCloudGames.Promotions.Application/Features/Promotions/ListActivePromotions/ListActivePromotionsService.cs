using FiapCloudGames.Promotions.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Promotions.Application.Features.Promotions.ListActivePromotions;

public sealed class ListActivePromotionsService(
    IPromotionRepository promotions,
    TimeProvider clock,
    ILogger<ListActivePromotionsService> logger)
{
    public async Task<IReadOnlyList<PromotionResult>> ExecuteAsync(
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Listando promoções ativas.");

        var result = (await promotions.ListActiveAsync(
            clock.GetUtcNow(),
            cancellationToken))
            .Select(PromotionApplicationMappings.ToResult)
            .ToArray();

        logger.LogDebug(
            "Listagem de promoções ativas concluída com {PromotionCount} itens.",
            result.Length);

        return result;
    }
}
