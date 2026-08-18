using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Catalog.Presentation.Features.Games.CreateGame;

/// <summary>
/// Representa a solicitação para criar um novo jogo.
/// </summary>
/// <param name="Title">Título do jogo.</param>
/// <param name="Description">Descrição do jogo.</param>
/// <param name="Category">Categoria do jogo.</param>
/// <param name="BasePrice">Preço base do jogo.</param>
public sealed record CreateGameRequest(
    [Required, StringLength(160, MinimumLength = 2)]
    string Title,

    [StringLength(4000)]
    string Description,

    [Required, StringLength(80)]
    string Category,

    [Range(0, double.MaxValue)]
    decimal BasePrice);
