using FiapCloudGames.Catalog.Application.Abstractions.Persistence;
using FiapCloudGames.Catalog.Application.Features.Games;
using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Catalog.Domain.Repositories;

namespace FiapCloudGames.Catalog.Application.Features.Games.CreateGame;

public sealed class CreateGameService(
    IGameRepository games,
    ICatalogUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task<GameResult> ExecuteAsync(
        CreateGameInput input,
        CancellationToken cancellationToken)
    {
        var game = Game.Create(
            input.Title,
            input.Description,
            input.Category,
            input.BasePrice,
            clock.GetUtcNow());
        await games.AddAsync(game, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return GameApplicationMappings.ToResult(game);
    }
}
