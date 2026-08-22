using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Application.Features.Promotions;
using FiapCloudGames.Promotions.Domain.Entities;
using FiapCloudGames.Promotions.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;

public sealed class CreatePromotionService(
    IPromotionRepository promotions,
    IPromotionsUnitOfWork unitOfWork,
    ICatalogModule catalog,
    TimeProvider clock,
    ILogger<CreatePromotionService> logger)
{
    public async Task<PromotionResult> ExecuteAsync(
        CreatePromotionInput input,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Iniciando criação de promoção para {GameCount} jogos.",
            input.GameIds.Count);

        var promotion = Promotion.Create(
            input.Name,
            input.DiscountPercent,
            input.StartsAtUtc,
            input.EndsAtUtc,
            input.GameIds,
            clock.GetUtcNow());

        foreach (var gameId in promotion.Games.Select(item => item.GameId))
        {
            logger.LogDebug(
                "Validando jogo {GameId} no módulo de catálogo para criação da promoção.",
                gameId);
            var game = await catalog.GetGameAsync(
                new GetGameQuery(gameId),
                cancellationToken);
            if (game is null)
            {
                logger.LogInformation(
                    "Falha ao criar promoção: jogo {GameId} não encontrado.",
                    gameId);
                throw AppException.NotFound(
                    $"O jogo '{gameId}' não foi encontrado.");
            }

            if (!game.IsActive)
            {
                logger.LogInformation(
                    "Falha ao criar promoção: jogo {GameId} está inativo.",
                    gameId);
                throw AppException.BusinessRule(
                    $"O jogo '{gameId}' está inativo.");
            }
        }

        await promotions.AddAsync(promotion, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Promoção {PromotionId} criada com sucesso para {GameCount} jogos.",
            promotion.Id,
            promotion.Games.Count);

        return PromotionApplicationMappings.ToResult(promotion);
    }
}
