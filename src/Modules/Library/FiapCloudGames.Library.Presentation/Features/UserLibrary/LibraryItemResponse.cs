namespace FiapCloudGames.Library.Presentation.Features.UserLibrary;

/// <summary>
/// Representa a resposta do item (jogo) da biblioteca.
/// </summary>
/// <param name="Id">O ID da biblioteca</param>
/// <param name="GameId">O ID do jogo.</param>
/// <param name="GameTitle">O título do jogo</param>
/// <param name="PricePaid">O valor pago no jogo</param>
/// <param name="PromotionId">O ID da promoção do jogo</param>
/// <param name="AcquiredAtUtc">A data de aquisição.</param>
public sealed record LibraryItemResponse(
    Guid Id,
    Guid GameId,
    string GameTitle,
    decimal PricePaid,
    Guid? PromotionId,
    DateTimeOffset AcquiredAtUtc);
