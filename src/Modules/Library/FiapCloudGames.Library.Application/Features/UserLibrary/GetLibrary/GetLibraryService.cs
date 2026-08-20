using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Library.Application.Abstractions.Queries;
using FiapCloudGames.Library.Application.Features.UserLibrary;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Library.Application.Features.UserLibrary.GetLibrary;

public sealed class GetLibraryService(
    ILibraryQueries queries,
    ICatalogModule catalog,
    ILogger<GetLibraryService> logger)
{
    public async Task<UserLibraryResult> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "Consultando biblioteca do usuário {UserId}.",
            userId);

        var libraryGames = await queries.ListGamesAsync(
            userId,
            cancellationToken);

        if (libraryGames.Count == 0)
        {
            logger.LogDebug(
                "Biblioteca do usuário {UserId} está vazia.",
                userId);
            return new UserLibraryResult(userId, []);
        }

        var items = new List<LibraryItemResult>(libraryGames.Count);
        foreach (var item in libraryGames)
        {
            var game = await catalog.GetGameAsync(
                new GetGameQuery(item.GameId),
                cancellationToken);

            if (game is null)
            {
                logger.LogWarning(
                    "Jogo {GameId} da biblioteca do usuário {UserId} não está disponível no catálogo.",
                    item.GameId,
                    userId);
            }

            items.Add(new LibraryItemResult(
                item.Id,
                item.GameId,
                game?.Title ?? "Jogo indisponível",
                item.PricePaid,
                item.PromotionId,
                item.AcquiredAtUtc));
        }

        logger.LogDebug(
            "Biblioteca do usuário {UserId} consultada com {GameCount} jogos.",
            userId,
            items.Count);

        return new UserLibraryResult(userId, items);
    }
}
