using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Identity.Contracts;
using FiapCloudGames.Library.Application.Abstractions.Persistence;
using FiapCloudGames.Library.Application.Features.UserLibrary;
using FiapCloudGames.Library.Domain.Entities;
using FiapCloudGames.Library.Domain.Repositories;
using FiapCloudGames.Promotions.Contracts;

namespace FiapCloudGames.Library.Application.Features.UserLibrary.AcquireGame;

public sealed class AcquireGameService(
    IGameLibraryRepository libraries,
    ILibraryUnitOfWork unitOfWork,
    IIdentityModule identity,
    ICatalogModule catalog,
    IPromotionsModule promotions,
    TimeProvider clock)
{
    public async Task<LibraryItemResult> ExecuteAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken)
    {
        var user = await identity.GetUserAsync(
            new GetUserQuery(userId),
            cancellationToken);
        if (user is null)
        {
            throw AppException.NotFound("Usuário não encontrado.");
        }

        if (!user.IsActive)
        {
            throw AppException.BusinessRule(
                "O usuário está inativo.");
        }

        var game = await catalog.GetGameAsync(
            new GetGameQuery(gameId),
            cancellationToken);
        if (game is null)
        {
            throw AppException.NotFound("Jogo não encontrado.");
        }

        if (!game.IsActive)
        {
            throw AppException.BusinessRule(
                "O jogo está inativo.");
        }

        var now = clock.GetUtcNow();
        var library = await libraries.GetByUserAsync(
            userId,
            cancellationToken);
        if (library is null)
        {
            library = GameLibrary.Create(userId, now);
            await libraries.AddAsync(library, cancellationToken);
        }

        if (library.ContainsGame(gameId))
        {
            throw AppException.Conflict(
                "O jogo já pertence à biblioteca do usuário.");
        }

        var quote = await promotions.GetPriceAsync(
            new GetPriceQuoteQuery(
                gameId,
                game.BasePrice),
            cancellationToken);
        var item = library.AcquireGame(
            gameId,
            quote.FinalPrice,
            quote.PromotionId,
            now);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LibraryItemResult(
            item.Id,
            item.GameId,
            game.Title,
            item.PricePaid.Amount,
            item.PromotionId,
            item.AcquiredAtUtc);
    }
}
