using FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;

namespace FiapCloudGames.Promotions.Presentation.Features.Promotions.CreatePromotion;

internal static class CreatePromotionHttpMappings
{
    public static CreatePromotionInput ToInput(
        this CreatePromotionRequest request) =>
        new(
            request.Name,
            request.DiscountPercent,
            request.StartsAtUtc,
            request.EndsAtUtc,
            request.GameIds);
}
