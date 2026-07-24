using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Identity.Contracts;
using FiapCloudGames.Library.Application.Abstractions;
using FiapCloudGames.Library.Contracts;
using FiapCloudGames.Library.Domain.Entities;
using FiapCloudGames.Library.Domain.Repositories;
using FiapCloudGames.Promotions.Contracts;

namespace FiapCloudGames.Library.Application.Games;

public sealed class AcquireGameService(
    IGameLibraryRepository libraries,
    ILibraryUnitOfWork unitOfWork,
    IIdentityModule identity,
    ICatalogModule catalog,
    IPromotionsModule promotions)
{
    public async Task<LibraryItemSummary> ExecuteAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken)
    {
        var user = await identity.GetUserAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new InvalidOperationException("O usuário não existe ou está inativo.");
        }

        var game = await catalog.GetGameAsync(gameId, cancellationToken);
        if (game is null || !game.IsActive)
        {
            throw new InvalidOperationException("O jogo não existe ou está inativo.");
        }

        var library = await libraries.GetByUserAsync(userId, trackChanges: true, cancellationToken);
        if (library is null)
        {
            library = GameLibrary.Create(userId);
            await libraries.AddAsync(library, cancellationToken);
        }

        var quote = await promotions.GetPriceAsync(gameId, game.BasePrice, cancellationToken);
        var item = library.AddGame(gameId, quote.FinalPrice, quote.PromotionId, DateTimeOffset.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LibraryItemSummary(
            item.Id,
            item.GameId,
            game.Title,
            item.PricePaid,
            item.PromotionId,
            item.AcquiredAtUtc);
    }
}

public sealed class GetLibraryService(IGameLibraryRepository libraries, ICatalogModule catalog)
{
    public async Task<UserLibrarySummary> ExecuteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var library = await libraries.GetByUserAsync(userId, trackChanges: false, cancellationToken);
        if (library is null)
        {
            return new UserLibrarySummary(userId, []);
        }

        var items = new List<LibraryItemSummary>(library.Games.Count);
        foreach (var item in library.Games.OrderByDescending(item => item.AcquiredAtUtc))
        {
            var game = await catalog.GetGameAsync(item.GameId, cancellationToken);
            items.Add(new LibraryItemSummary(
                item.Id,
                item.GameId,
                game?.Title ?? "Jogo indisponível",
                item.PricePaid,
                item.PromotionId,
                item.AcquiredAtUtc));
        }

        return new UserLibrarySummary(userId, items);
    }
}

internal sealed class LibraryModule(GetLibraryService service) : ILibraryModule
{
    public Task<UserLibrarySummary> GetLibraryAsync(Guid userId, CancellationToken cancellationToken) =>
        service.ExecuteAsync(userId, cancellationToken);
}
