using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Catalog.Application.Features.Games.FindGame;

public sealed class FindGameService(
    IGameRepository games,
    ILogger<FindGameService> logger)
{
    public async Task<GameResult?> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "Consultando jogo {GameId}.",
            id);

        var game = await games.GetAsync(id, cancellationToken);

        if (game is null)
        {
            logger.LogDebug(
                "Jogo {GameId} não encontrado.",
                id);
            return null;
        }

        return GameApplicationMappings.ToResult(game);
    }
}
