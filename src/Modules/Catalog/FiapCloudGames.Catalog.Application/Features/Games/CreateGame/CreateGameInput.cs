namespace FiapCloudGames.Catalog.Application.Features.Games.CreateGame;

public sealed record CreateGameInput(
    string Title,
    string Description,
    string Category,
    decimal BasePrice);
