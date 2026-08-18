using FiapCloudGames.Catalog.Domain.Repositories;

namespace FiapCloudGames.Catalog.Application.Features.Games.ListGames;

public sealed class ListGamesService(IGameRepository games)
{
    public async Task<IReadOnlyList<GameResult>> ExecuteAsync(
        bool onlyActive,
        CancellationToken cancellationToken)
    {
        var result = (await games.ListAsync(onlyActive, cancellationToken))
            .Select(GameApplicationMappings.ToResult)
            .ToArray();

        return result;
    }
}

