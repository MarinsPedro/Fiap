using FiapCloudGames.Catalog.Domain.Repositories;

namespace FiapCloudGames.Catalog.Application.Features.Games.GetGame;

public sealed class GetGameService(IGameRepository games)
{
    public async Task<GameResult?> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var game = await games.GetAsync(id, cancellationToken);

        return game is null
            ? null
            : GameApplicationMappings.ToResult(game);
    }
}
