using FiapCloudGames.Promotions.Application.Features.Promotions;

namespace FiapCloudGames.Promotions.Presentation.Features.Promotions;

internal static class PromotionResponseMappings
{
    public static PromotionResponse ToResponse(
        this PromotionResult result) =>
        new(
            result.Id,
            result.Name,
            result.DiscountPercent,
            result.StartsAtUtc,
            result.EndsAtUtc,
            result.GameIds);
}
