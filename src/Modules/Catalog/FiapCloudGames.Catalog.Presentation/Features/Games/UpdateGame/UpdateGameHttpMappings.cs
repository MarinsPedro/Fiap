using FiapCloudGames.Catalog.Application.Features.Games.UpdateGame;

namespace FiapCloudGames.Catalog.Presentation.Features.Games.UpdateGame;

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
