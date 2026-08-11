using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Promotions.Presentation.Features.Promotions.CreatePromotion;

public sealed record CreatePromotionRequest(
    [Required, StringLength(120, MinimumLength = 2)]
    string Name,

    [Range(0d, 100d, MinimumIsExclusive = true)]
    decimal DiscountPercent,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,

    [Required, MinLength(1)]
    IReadOnlyCollection<Guid> GameIds);
