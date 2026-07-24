using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Promotions.Application.Abstractions;
using FiapCloudGames.Promotions.Contracts;
using FiapCloudGames.Promotions.Domain.Entities;
using FiapCloudGames.Promotions.Domain.Repositories;

namespace FiapCloudGames.Promotions.Application.Promotions;

public sealed record CreatePromotionInput(
    string Name,
    decimal DiscountPercent,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    IReadOnlyCollection<Guid> GameIds);

public sealed class CreatePromotionService(
    IPromotionRepository promotions,
    IPromotionsUnitOfWork unitOfWork,
    ICatalogModule catalog)
{
    public async Task<PromotionSummary> ExecuteAsync(
        CreatePromotionInput input,
        CancellationToken cancellationToken)
    {
        foreach (var gameId in input.GameIds.Distinct())
        {
            var game = await catalog.GetGameAsync(gameId, cancellationToken);
            if (game is null || !game.IsActive)
            {
                throw new InvalidOperationException($"O jogo '{gameId}' não existe ou está inativo.");
            }
        }

        var promotion = Promotion.Create(
            input.Name,
            input.DiscountPercent,
            input.StartsAtUtc,
            input.EndsAtUtc,
            input.GameIds);
        await promotions.AddAsync(promotion, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return PromotionMappings.ToSummary(promotion);
    }
}

public sealed class ListActivePromotionsService(IPromotionRepository promotions)
{
    public async Task<IReadOnlyList<PromotionSummary>> ExecuteAsync(CancellationToken cancellationToken) =>
        (await promotions.ListActiveAsync(DateTimeOffset.UtcNow, cancellationToken))
            .Select(PromotionMappings.ToSummary)
            .ToArray();
}

public sealed class EndPromotionService(IPromotionRepository promotions, IPromotionsUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var promotion = await promotions.GetAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Promoção não encontrada.");
        promotion.End(DateTimeOffset.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class GetPromotionalPriceService(IPromotionRepository promotions)
{
    public async Task<PriceQuote> ExecuteAsync(
        Guid gameId,
        decimal basePrice,
        CancellationToken cancellationToken)
    {
        var promotion = await promotions.GetActiveForGameAsync(gameId, DateTimeOffset.UtcNow, cancellationToken);
        return promotion is null
            ? new PriceQuote(basePrice, basePrice, 0, null)
            : new PriceQuote(basePrice, promotion.ApplyTo(basePrice), promotion.DiscountPercent, promotion.Id);
    }
}

internal sealed class PromotionsModule(GetPromotionalPriceService service) : IPromotionsModule
{
    public Task<PriceQuote> GetPriceAsync(Guid gameId, decimal basePrice, CancellationToken cancellationToken) =>
        service.ExecuteAsync(gameId, basePrice, cancellationToken);
}

internal static class PromotionMappings
{
    public static PromotionSummary ToSummary(Promotion promotion) =>
        new(
            promotion.Id,
            promotion.Name,
            promotion.DiscountPercent,
            promotion.StartsAtUtc,
            promotion.EndsAtUtc,
            promotion.Games.Select(item => item.GameId).ToArray());
}
