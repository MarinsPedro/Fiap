using FiapCloudGames.Catalog.Application.Features.Games;

namespace FiapCloudGames.Catalog.Presentation.Features.Games;

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
