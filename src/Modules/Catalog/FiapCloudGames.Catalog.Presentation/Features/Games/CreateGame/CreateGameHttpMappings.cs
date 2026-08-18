using FiapCloudGames.Catalog.Application.Features.Games.CreateGame;

namespace FiapCloudGames.Catalog.Presentation.Features.Games.CreateGame;

/// <summary>
/// Mapeamentos para conversão entre os objetos de request e o objeto input do serviço de criação de jogos.
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
