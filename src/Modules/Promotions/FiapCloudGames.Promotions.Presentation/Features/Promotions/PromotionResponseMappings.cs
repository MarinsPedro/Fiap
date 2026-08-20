using FiapCloudGames.Promotions.Application.Features.Promotions;

namespace FiapCloudGames.Promotions.Presentation.Features.Promotions;

/// <summary>
/// Fornece métodos de extensão para mapear objetos de resultado de promoção para objetos de resposta de promoção.
/// </summary>
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
