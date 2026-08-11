using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Application.Features.Promotions;
using FiapCloudGames.Promotions.Domain.Entities;
using FiapCloudGames.Promotions.Domain.Repositories;

namespace FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;

public sealed class CreatePromotionService(
    IPromotionRepository promotions,
    IPromotionsUnitOfWork unitOfWork,
    ICatalogModule catalog,
    TimeProvider clock)
{
    public async Task<PromotionResult> ExecuteAsync(
        CreatePromotionInput input,
        CancellationToken cancellationToken)
    {
        var promotion = Promotion.Create(
            input.Name,
            input.DiscountPercent,
            input.StartsAtUtc,
            input.EndsAtUtc,
            input.GameIds,
            clock.GetUtcNow());

        foreach (var gameId in promotion.Games.Select(item => item.GameId))
        {
            var game = await catalog.GetGameAsync(
                new GetGameQuery(gameId),
                cancellationToken);
            if (game is null)
            {
                throw AppException.NotFound(
                    $"O jogo '{gameId}' não foi encontrado.");
            }

            if (!game.IsActive)
            {
                throw AppException.BusinessRule(
                    $"O jogo '{gameId}' está inativo.");
            }
        }

        await promotions.AddAsync(promotion, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return PromotionApplicationMappings.ToResult(promotion);
    }
}
