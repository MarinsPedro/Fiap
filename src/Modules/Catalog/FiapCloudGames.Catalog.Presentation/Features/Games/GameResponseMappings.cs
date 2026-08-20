using FiapCloudGames.Catalog.Application.Features.Games;

namespace FiapCloudGames.Catalog.Presentation.Features.Games;

/// <summary>
/// Classe de mapeamento para conversão de resultados de jogos em respostas de API.
/// </summary>
internal static class GameResponseMappings
{
    public static GameResponse ToResponse(
        this GameResult result) =>
        new(
            result.Id,
            result.Title,
            result.Description,
            result.Category,
            result.BasePrice,
            result.IsActive);
}
