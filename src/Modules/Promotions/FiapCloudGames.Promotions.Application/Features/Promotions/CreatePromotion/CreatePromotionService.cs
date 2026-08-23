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

        var gameIds = promotion.Games
            .Select(item => item.GameId)
            .ToArray();

        logger.LogDebug(
            "Validando {GameCount} jogos no módulo de catálogo para criação da promoção.",
            gameIds.Length);
        var catalogGames = await catalog.GetGamesAsync(
            new GetGamesQuery(gameIds),
            cancellationToken);
        var catalogGamesById = catalogGames.ToDictionary(game => game.Id);

        foreach (var gameId in gameIds)
        {
            if (!catalogGamesById.TryGetValue(gameId, out var game))
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
