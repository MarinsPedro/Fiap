using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Catalog.Presentation.Features.Games.CreateGame;

public sealed record CreateGameRequest(
    [Required, StringLength(160, MinimumLength = 2)]
    string Title,

    [StringLength(4000)]
    string Description,

    [Required, StringLength(80)]
    string Category,

    [Range(0, double.MaxValue)]
    decimal BasePrice);
