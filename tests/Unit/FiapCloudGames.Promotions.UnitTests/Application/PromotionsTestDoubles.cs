using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Entities;
using FiapCloudGames.Promotions.Domain.Repositories;

namespace FiapCloudGames.Promotions.UnitTests.Application;

internal static class PromotionsTestData
{
    internal static readonly DateTimeOffset NowUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    internal static Promotion CreatePromotion(
        Guid? gameId = null,
        decimal discountPercent = 25m) =>
        Promotion.Create(
            "FIAP Week",
            discountPercent,
            NowUtc.AddHours(-1),
            NowUtc.AddHours(1),
            [gameId ?? Guid.NewGuid()],
            NowUtc.AddHours(-2));
}

internal sealed class FakePromotionRepository : IPromotionRepository
{
    internal Promotion? Promotion { get; set; }
    internal Promotion? ActivePromotion { get; set; }
    internal IReadOnlyList<Promotion> ActivePromotions { get; set; } = [];
    internal Promotion? AddedPromotion { get; private set; }
    internal Guid? RequestedId { get; private set; }
    internal Guid? RequestedGameId { get; private set; }
    internal DateTimeOffset? RequestedInstant { get; private set; }
    internal int AddCalls { get; private set; }

    public Task AddAsync(
        Promotion promotion,
        CancellationToken cancellationToken)
    {
        AddCalls++;
        AddedPromotion = promotion;
        return Task.CompletedTask;
    }

    public Task<Promotion?> GetAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        RequestedId = id;
        return Task.FromResult(
            Promotion?.Id == id
                ? Promotion
                : null);
    }

    public Task<Promotion?> GetActiveForGameAsync(
        Guid gameId,
        DateTimeOffset instant,
        CancellationToken cancellationToken)
    {
        RequestedGameId = gameId;
        RequestedInstant = instant;
        return Task.FromResult(ActivePromotion);
    }

    public Task<IReadOnlyList<Promotion>> ListActiveAsync(
        DateTimeOffset instant,
        CancellationToken cancellationToken)
    {
        RequestedInstant = instant;
        return Task.FromResult(ActivePromotions);
    }
}

internal sealed class StubCatalogModule : ICatalogModule
{
    internal IReadOnlyList<GameSnapshot> GamesResult { get; set; } = [];
    internal IReadOnlyCollection<Guid> RequestedIds { get; private set; } = [];
    internal int BatchCalls { get; private set; }

    public Task<GameSnapshot?> GetGameAsync(
        GetGameQuery query,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<GameSnapshot>> GetGamesAsync(
        GetGamesQuery query,
        CancellationToken cancellationToken)
    {
        BatchCalls++;
        RequestedIds = query.GameIds;
        return Task.FromResult(GamesResult);
    }
}

internal sealed class SpyPromotionsUnitOfWork : IPromotionsUnitOfWork
{
    internal int SaveChangesCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset nowUtc) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => nowUtc;
}
