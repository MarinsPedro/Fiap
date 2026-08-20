using FiapCloudGames.Catalog.Application.Abstractions.Persistence;
using FiapCloudGames.Catalog.Application.Features.Games;
using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Catalog.Application.Features.Games.CreateGame;

public sealed class CreateGameService(
    IGameRepository games,
    ICatalogUnitOfWork unitOfWork,
    TimeProvider clock,
    ILogger<CreateGameService> logger)
{
    public async Task<GameResult> ExecuteAsync(
        CreateGameInput input,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando criação de jogo.");

        var game = Game.Create(
            input.Title,
            input.Description,
            input.Category,
            input.BasePrice,
            clock.GetUtcNow());
        await games.AddAsync(game, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Jogo {GameId} criado com sucesso.",
            game.Id);

        return GameApplicationMappings.ToResult(game);
    }
}
