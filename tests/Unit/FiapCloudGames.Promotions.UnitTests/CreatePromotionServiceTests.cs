using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;
using FiapCloudGames.Promotions.Domain.Entities;
using FiapCloudGames.Promotions.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Promotions.UnitTests;

public sealed class CreatePromotionServiceTests
{
    private static readonly DateTimeOffset NowUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CreatePromotionShouldValidateCatalogGamesInSingleBatch()
    {
        var gameIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var catalog = new CatalogModule(
            gameIds.Select(id =>
                new GameSnapshot(id, $"Jogo {id}", 10m, true))
                .ToArray());
        var promotions = new PromotionRepository();
        var unitOfWork = new PromotionsUnitOfWork();
        var service = new CreatePromotionService(
            promotions,
            unitOfWork,
            catalog,
            new Clock(NowUtc),
            NullLogger<CreatePromotionService>.Instance);

        var result = await service.ExecuteAsync(
            new CreatePromotionInput(
                "FIAP Week",
                10m,
                NowUtc.AddHours(1),
                NowUtc.AddHours(2),
                gameIds),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(1, catalog.BatchCallCount);
        Assert.Equal(0, catalog.SingleCallCount);
        Assert.Equal(gameIds.Order(), catalog.RequestedIds.Order());
        Assert.NotNull(promotions.AddedPromotion);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    private sealed class CatalogModule(
        IReadOnlyList<GameSnapshot> games) : ICatalogModule
    {
        public int SingleCallCount { get; private set; }
        public int BatchCallCount { get; private set; }
        public IReadOnlyCollection<Guid> RequestedIds { get; private set; } = [];

        public Task<GameSnapshot?> GetGameAsync(
            GetGameQuery query,
            CancellationToken cancellationToken)
        {
            SingleCallCount++;
            throw new InvalidOperationException(
                "A consulta individual não deveria ser executada.");
        }

        public Task<IReadOnlyList<GameSnapshot>> GetGamesAsync(
            GetGamesQuery query,
            CancellationToken cancellationToken)
        {
            BatchCallCount++;
            RequestedIds = query.GameIds;
            return Task.FromResult(games);
        }
    }

    private sealed class PromotionRepository : IPromotionRepository
    {
        public Promotion? AddedPromotion { get; private set; }

        public Task AddAsync(
            Promotion promotion,
            CancellationToken cancellationToken)
        {
            AddedPromotion = promotion;
            return Task.CompletedTask;
        }

        public Task<Promotion?> GetAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Promotion?> GetActiveForGameAsync(
            Guid gameId,
            DateTimeOffset instant,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<Promotion>> ListActiveAsync(
            DateTimeOffset instant,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class PromotionsUnitOfWork : IPromotionsUnitOfWork
    {
        public int SaveChangesCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            SaveChangesCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class Clock(DateTimeOffset nowUtc) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => nowUtc;
    }
}
