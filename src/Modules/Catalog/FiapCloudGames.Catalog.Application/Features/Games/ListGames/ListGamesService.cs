using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Catalog.Application.Features.Games.ListGames;

public sealed class ListGamesService(
    IGameRepository games,
    ILogger<ListGamesService> logger)
{
    public async Task<IReadOnlyList<GameResult>> ExecuteAsync(
        bool onlyActive,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "Listando jogos. Apenas ativos: {OnlyActive}.",
            onlyActive);

        var result = (await games.ListAsync(onlyActive, cancellationToken))
            .Select(GameApplicationMappings.ToResult)
            .ToArray();

        logger.LogDebug(
            "Listagem de jogos concluída com {GameCount} itens.",
            result.Length);

        return result;
    }
}

