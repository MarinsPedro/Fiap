using FiapCloudGames.Catalog.Application.Features.Games;
using FiapCloudGames.Catalog.Domain.Repositories;

namespace FiapCloudGames.Catalog.Application.Features.Games.ListGames;

public sealed class ListGamesService(IGameRepository games)
{
    public async Task<IReadOnlyList<GameResult>> ExecuteAsync(
        bool onlyActive,
        CancellationToken cancellationToken) =>
        (await games.ListAsync(onlyActive, cancellationToken))
            .Select(GameApplicationMappings.ToResult)
            .ToArray();
}
