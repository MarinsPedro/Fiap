using FiapCloudGames.Application.Common.Authentication;
using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Identity.Contracts;
using FiapCloudGames.Library.Application.Abstractions.Persistence;
using FiapCloudGames.Library.Domain.Entities;
using FiapCloudGames.Library.Domain.Repositories;
using FiapCloudGames.Promotions.Contracts;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Library.Application.Features.UserLibrary.AcquireGame;

public sealed class AcquireGameService(
    ICurrentUserContext currentUser,
    IGameLibraryRepository libraries,
    ILibraryUnitOfWork unitOfWork,
    IIdentityModule identity,
    ICatalogModule catalog,
    IPromotionsModule promotions,
    TimeProvider clock,
    ILogger<AcquireGameService> logger)
{
    public async Task<LibraryItemResult> ExecuteAsync(
        Guid gameId,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.GetRequiredUserId();

        logger.LogInformation(
            "Iniciando aquisição do jogo {GameId} para o usuário {UserId}.",
            gameId,
            userId);

        logger.LogDebug(
            "Validando usuário {UserId} no módulo de identidade.",
            userId);
        var user = await identity.GetUserAsync(
            new GetUserQuery(userId),
            cancellationToken);
        if (user is null)
        {
            logger.LogInformation(
                "Falha ao adquirir o jogo {GameId}: usuário {UserId} não encontrado.",
                gameId,
                userId);
            throw AppException.NotFound(
                "Usuário não encontrado.");
        }

        if (!user.IsActive)
        {
            logger.LogInformation(
                "Falha ao adquirir o jogo {GameId}: usuário {UserId} está inativo.",
                gameId,
                userId);
            throw AppException.BusinessRule(
                "O usuário está inativo.");
        }

        logger.LogDebug(
            "Validando jogo {GameId} no módulo de catálogo.",
            gameId);
        var game = await catalog.GetGameAsync(
            new GetGameQuery(gameId),
            cancellationToken);
        if (game is null)
        {
            logger.LogInformation(
                "Falha ao adquirir o jogo {GameId}: jogo não encontrado para o usuário {UserId}.",
                gameId,
                userId);
            throw AppException.NotFound(
                "Jogo não encontrado.");
        }

        if (!game.IsActive)
        {
            logger.LogInformation(
                "Falha ao adquirir o jogo {GameId}: jogo inativo para o usuário {UserId}.",
                gameId,
                userId);
            throw AppException.BusinessRule(
                "O jogo está inativo.");
        }

        var now = clock.GetUtcNow();
        var library = await libraries.GetByUserAsync(userId, cancellationToken);

        if (library is null)
        {
            library = GameLibrary.Create(userId, now);
            await libraries.AddAsync(library, cancellationToken);

            logger.LogDebug(
                "Biblioteca criada para o usuário {UserId}.",
                userId);
        }

        if (library.ContainsGame(gameId))
        {
            logger.LogInformation(
                "Falha ao adquirir o jogo {GameId}: jogo já pertence ao usuário {UserId}.",
                gameId,
                userId);
            throw AppException.Conflict(
                "O jogo já pertence à biblioteca do usuário.");
        }

        logger.LogDebug(
            "Consultando preço do jogo {GameId} no módulo de promoções.",
            gameId);
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

        logger.LogInformation(
            "Jogo {GameId} adquirido pelo usuário {UserId}. Item {LibraryItemId}, promoção {PromotionId}.",
            gameId,
            userId,
            item.Id,
            item.PromotionId);

        return new LibraryItemResult(
            item.Id,
            item.GameId,
            game.Title,
            item.PricePaid.Amount,
            item.PromotionId,
            item.AcquiredAtUtc);
    }
}
