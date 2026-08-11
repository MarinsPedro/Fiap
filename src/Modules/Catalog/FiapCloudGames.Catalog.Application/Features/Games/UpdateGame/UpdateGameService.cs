using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Application.Abstractions.Persistence;
using FiapCloudGames.Catalog.Application.Features.Games;
using FiapCloudGames.Catalog.Domain.Repositories;

namespace FiapCloudGames.Catalog.Application.Features.Games.UpdateGame;

public sealed class UpdateGameService(
    IGameRepository games,
    ICatalogUnitOfWork unitOfWork)
{
    public async Task<GameResult> ExecuteAsync(
        Guid id,
        UpdateGameInput input,
        CancellationToken cancellationToken)
    {
        var game = await games.GetAsync(id, cancellationToken)
            ?? throw AppException.NotFound("Jogo não encontrado.");
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
        return GameApplicationMappings.ToResult(game);
    }
}
