using FiapCloudGames.Catalog.Application.Features.Games;
using FiapCloudGames.Catalog.Application.Features.Games.FindGame;
using FiapCloudGames.Catalog.Application.Features.Games.FindGames;
using FiapCloudGames.Catalog.Contracts;

namespace FiapCloudGames.Catalog.Application.Integrations;

internal sealed class CatalogModule(
    FindGameService findGame,
    FindGamesService findGames)
    : ICatalogModule
{
    public async Task<GameSnapshot?> GetGameAsync(
        GetGameQuery query,
        CancellationToken cancellationToken)
    {
        var result = await findGame.ExecuteAsync(
            query.GameId,
            cancellationToken);

        return result is null
            ? null
            : ToSnapshot(result);
    }

    public async Task<IReadOnlyList<GameSnapshot>> GetGamesAsync(
        GetGamesQuery query,
        CancellationToken cancellationToken)
    {
        var results = await findGames.ExecuteAsync(
            query.GameIds,
            cancellationToken);

        return results.Select(ToSnapshot).ToArray();
    }

    private static GameSnapshot ToSnapshot(GameResult result) =>
        new(
            result.Id,
            result.Title,
            result.BasePrice,
            result.IsActive);
}
