namespace FiapCloudGames.Promotions.Presentation.Features.Promotions;

/// <summary>
/// Representa a resposta de uma promoção.
/// </summary>
/// <param name="Id">O identificador único da promoção.</param>
/// <param name="Name">O nome da promoção.</param>
/// <param name="DiscountPercent">A porcentagem de desconto da promoção.</param>
/// <param name="StartsAtUtc">A data e hora de início da promoção em UTC.</param>
/// <param name="EndsAtUtc">A data e hora de término da promoção em UTC.</param>
/// <param name="GameIds">A coleção de identificadores únicos dos jogos associados à promoção.</param>
public sealed record PromotionResponse(
    Guid Id,
    string Name,
    decimal DiscountPercent,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    IReadOnlyCollection<Guid> GameIds);
