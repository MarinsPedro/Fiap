using FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;

namespace FiapCloudGames.Promotions.Presentation.Features.Promotions.CreatePromotion;

/// <summary>
/// Fornece mapeamentos de entrada para a criação de promoções.
/// </summary>
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
