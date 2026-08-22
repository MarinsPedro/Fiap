using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Application.Abstractions.Persistence;
using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Catalog.Application.Features.Games.DeactivateGame;

public sealed class DeactivateGameService(
    IGameRepository gameRepository,
    ICatalogUnitOfWork unitOfWork,
    ILogger<DeactivateGameService> logger)
{
    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando desativação do jogo {GameId}.", id);

        var game = await gameRepository.GetAsync(id, cancellationToken);

        if (game is null)
        {
            logger.LogInformation("Não foi possível desativar: jogo {GameId} não encontrado.", id);
            throw AppException.NotFound(
                "Jogo não encontrado.");
        }

        if(!game.IsActive)
        {
            logger.LogInformation("Não foi possível desativar: jogo {GameId} já está desativado.", id);
            throw AppException.Conflict(
                "Jogo já está desativado.");
        }

        game.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Jogo {GameId} desativado com sucesso.", game.Id);
    }
}
