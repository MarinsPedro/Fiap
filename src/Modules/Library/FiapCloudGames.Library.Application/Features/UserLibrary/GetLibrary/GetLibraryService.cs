using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Library.Application.Abstractions.Queries;
using FiapCloudGames.Library.Application.Features.UserLibrary;

namespace FiapCloudGames.Library.Application.Features.UserLibrary.GetLibrary;

public sealed class GetLibraryService(
    ILibraryQueries queries,
    ICatalogModule catalog)
{
    public async Task<UserLibraryResult> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var libraryGames = await queries.ListGamesAsync(
            userId,
            cancellationToken);

        if (libraryGames.Count == 0)
        {
            return new UserLibraryResult(userId, []);
        }

        var items = new List<LibraryItemResult>(libraryGames.Count);
        foreach (var item in libraryGames)
        {
            var game = await catalog.GetGameAsync(
                new GetGameQuery(item.GameId),
                cancellationToken);
            items.Add(new LibraryItemResult(
                item.Id,
                item.GameId,
                game?.Title ?? "Jogo indisponível",
                item.PricePaid,
                item.PromotionId,
                item.AcquiredAtUtc));
        }

        return new UserLibraryResult(userId, items);
    }
}
