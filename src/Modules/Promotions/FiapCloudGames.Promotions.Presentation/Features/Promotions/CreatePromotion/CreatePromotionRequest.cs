using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Promotions.Presentation.Features.Promotions.CreatePromotion;

/// <summary>
/// Representa a solicitação para criar uma nova promoção.
/// </summary>
/// <param name="Name">O nome da promoção.</param>
/// <param name="DiscountPercent">A porcentagem de desconto da promoção.</param>
/// <param name="StartsAtUtc">A data e hora de início da promoção em UTC.</param>
/// <param name="EndsAtUtc">A data e hora de término da promoção em UTC.</param>
/// <param name="GameIds">A coleção de identificadores únicos dos jogos associados à promoção.</param>
public sealed record CreatePromotionRequest(
    [Required, StringLength(120, MinimumLength = 2)]
    string Name,

    [Range(0d, 100d, MinimumIsExclusive = true)]
    decimal DiscountPercent,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,

    [Required, MinLength(1)]
    IReadOnlyCollection<Guid> GameIds);
