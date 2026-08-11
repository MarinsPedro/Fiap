namespace FiapCloudGames.Catalog.Application.Features.Games.UpdateGame;

public sealed record UpdateGameInput(
    string Title,
    string Description,
    string Category,
    decimal BasePrice,
    bool IsActive);
