using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Catalog.Application.Features.Games.FindGames;

public sealed class FindGamesService(
    IGameRepository games,
    ILogger<FindGamesService> logger)
{
    public async Task<IReadOnlyList<GameResult>> ExecuteAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "Consultando {GameCount} jogos em lote.",
            ids.Count);

        var results = await games.ListByIdsAsync(ids, cancellationToken);

        return results
            .Select(GameApplicationMappings.ToResult)
            .ToArray();
    }
}
