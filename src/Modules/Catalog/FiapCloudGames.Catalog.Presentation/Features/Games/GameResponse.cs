namespace FiapCloudGames.Catalog.Presentation.Features.Games;

/// <summary>
/// Representa a resposta de um jogo.
/// </summary>
/// <param name="Id">Identificador do jogo.</param>
/// <param name="Title">Título do jogo.</param>
/// <param name="Description">Descrição do jogo.</param>
/// <param name="Category">Categoria do jogo.</param>
/// <param name="BasePrice">Preço base do jogo.</param>
/// <param name="IsActive">Indica se o jogo está ativo.</param>
public sealed record GameResponse(
    Guid Id,
    string Title,
    string Description,
    string Category,
    decimal BasePrice,
    bool IsActive);
