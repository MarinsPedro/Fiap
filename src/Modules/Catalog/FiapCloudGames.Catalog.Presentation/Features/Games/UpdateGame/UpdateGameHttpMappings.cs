using FiapCloudGames.Catalog.Application.Features.Games.UpdateGame;

namespace FiapCloudGames.Catalog.Presentation.Features.Games.UpdateGame;

/// <summary>
/// Classe de mapeamento para a atualização de jogos via HTTP.
/// </summary>
internal static class UpdateGameHttpMappings
{
    public static UpdateGameInput ToInput(
        this UpdateGameRequest request) =>
        new(
            request.Title,
            request.Description,
            request.Category,
            request.BasePrice,
            request.IsActive);
}
