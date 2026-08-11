using FiapCloudGames.Catalog.Application.Features.Games.GetGame;
using FiapCloudGames.Catalog.Contracts;

namespace FiapCloudGames.Catalog.Application.Integrations;

internal sealed class CatalogModule(GetGameService getGameService)
    : ICatalogModule
{
    public async Task<GameSnapshot?> GetGameAsync(
        GetGameQuery query,
        CancellationToken cancellationToken)
    {
        var result = await getGameService.ExecuteAsync(
            query.GameId,
            cancellationToken);

        return result is null
            ? null
            : new GameSnapshot(
                result.Id,
                result.Title,
                result.BasePrice,
                result.IsActive);
    }
}
