using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Application.Abstractions.Persistence;
using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Catalog.Application.Features.Games.UpdateGame;

public sealed class UpdateGameService(
    IGameRepository games,
    ICatalogUnitOfWork unitOfWork,
    ILogger<UpdateGameService> logger)
{
    public async Task<GameResult> ExecuteAsync(
        Guid id,
        UpdateGameInput input,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Iniciando atualização do jogo {GameId}.",
            id);

        var game = await games.GetAsync(id, cancellationToken);
        if (game is null)
        {
            logger.LogWarning(
                "Não foi possível atualizar: jogo {GameId} não encontrado.",
                id);
            throw AppException.NotFound("Jogo não encontrado.");
        }

        game.ChangeDetails(
            input.Title,
            input.Description,
            input.Category,
            input.BasePrice);

        if (input.IsActive)
        {
            game.Activate();
        }
        else
        {
            game.Deactivate();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Jogo {GameId} atualizado com sucesso. Ativo: {IsActive}.",
            game.Id,
            game.IsActive);

        return GameApplicationMappings.ToResult(game);
    }
}
