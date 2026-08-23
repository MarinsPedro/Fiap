using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Application.Features.Games.FindGame;

namespace FiapCloudGames.Catalog.Application.Features.Games.GetGame;

public sealed class GetGameService(FindGameService findGame)
{
    public async Task<GameResult> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var game = await findGame.ExecuteAsync(id, cancellationToken);
        return game ?? throw AppException.NotFound("Jogo não encontrado.");
    }
}
