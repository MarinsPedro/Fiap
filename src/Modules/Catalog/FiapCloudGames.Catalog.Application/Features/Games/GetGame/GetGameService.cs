using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Catalog.Application.Features.Games.GetGame;

public sealed class GetGameService(
    IGameRepository games,
    ILogger<GetGameService> logger)
{
    public async Task<GameResult> ExecuteAsync(
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
            throw AppException.NotFound("Jogo não encontrado.");
        }

        return GameApplicationMappings.ToResult(game);
    }
}
