using FiapCloudGames.Promotions.Domain.Entities;

namespace FiapCloudGames.Promotions.Application.Features.Promotions;

internal static class PromotionApplicationMappings
{
    public static PromotionResult ToResult(Promotion promotion) =>
        new(
            promotion.Id,
            promotion.Name,
            promotion.DiscountPercent.Value,
            promotion.StartsAtUtc,
            promotion.EndsAtUtc,
            promotion.Games
                .Select(item => item.GameId)
                .ToArray());
}
