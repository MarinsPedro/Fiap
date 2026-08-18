using FiapCloudGames.Catalog.Application.Features.Games.CreateGame;

namespace FiapCloudGames.Catalog.Presentation.Features.Games.CreateGame;

/// <summary>
/// Classe de mapeamento para a criação de jogos via HTTP.
/// </summary>
internal static class CreateGameHttpMappings
{
    public static CreateGameInput ToInput(
        this CreateGameRequest request) =>
        new(
            request.Title,
            request.Description,
            request.Category,
            request.BasePrice);
}
