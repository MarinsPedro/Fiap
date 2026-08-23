using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Domain.Common;
using FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Promotions.UnitTests.Application;

public sealed class CreatePromotionServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithActiveGames_ShouldCreateAndPersistPromotion()
    {
        var gameIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var catalog = new StubCatalogModule
        {
            GamesResult = gameIds
                .Select(id => new GameSnapshot(id, $"Jogo {id}", 10m, true))
                .ToArray()
        };
        var promotions = new FakePromotionRepository();
        var unitOfWork = new SpyPromotionsUnitOfWork();
        var service = CreateService(promotions, unitOfWork, catalog);

        var result = await service.ExecuteAsync(
            CreateInput(gameIds),
            CancellationToken.None);

        var addedPromotion = Assert.IsType<Promotions.Domain.Entities.Promotion>(
            promotions.AddedPromotion);
        Assert.Equal(1, catalog.BatchCalls);
        Assert.Equal(gameIds.Order(), catalog.RequestedIds.Order());
        Assert.Equal(1, promotions.AddCalls);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
        Assert.Equal(addedPromotion.Id, result.Id);
        Assert.Equal(gameIds.Order(), result.GameIds.Order());
    }

    [Fact]
    public async Task ExecuteAsync_WhenCatalogGameDoesNotExist_ShouldThrowNotFoundWithoutPersisting()
    {
        var existingGameId = Guid.NewGuid();
        var missingGameId = Guid.NewGuid();
        var catalog = new StubCatalogModule
        {
            GamesResult =
            [
                new GameSnapshot(existingGameId, "Existente", 10m, true)
            ]
        };
        var promotions = new FakePromotionRepository();
        var unitOfWork = new SpyPromotionsUnitOfWork();
        var service = CreateService(promotions, unitOfWork, catalog);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                CreateInput([existingGameId, missingGameId]),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Contains(missingGameId.ToString(), exception.Message);
        Assert.Null(promotions.AddedPromotion);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCatalogGameIsInactive_ShouldThrowBusinessRuleWithoutPersisting()
    {
        var gameId = Guid.NewGuid();
        var catalog = new StubCatalogModule
        {
            GamesResult =
            [
                new GameSnapshot(gameId, "Inativo", 10m, false)
            ]
        };
        var promotions = new FakePromotionRepository();
        var unitOfWork = new SpyPromotionsUnitOfWork();
        var service = CreateService(promotions, unitOfWork, catalog);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                CreateInput([gameId]),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.BusinessRule, exception.Category);
        Assert.Contains(gameId.ToString(), exception.Message);
        Assert.Null(promotions.AddedPromotion);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPeriod_ShouldNotCallCatalogOrPersist()
    {
        var catalog = new StubCatalogModule();
        var promotions = new FakePromotionRepository();
        var unitOfWork = new SpyPromotionsUnitOfWork();
        var service = CreateService(promotions, unitOfWork, catalog);
        var gameId = Guid.NewGuid();
        var input = new CreatePromotionInput(
            "FIAP Week",
            10m,
            PromotionsTestData.NowUtc,
            PromotionsTestData.NowUtc,
            [gameId]);

        await Assert.ThrowsAsync<DomainRuleViolationException>(() =>
            service.ExecuteAsync(input, CancellationToken.None));

        Assert.Equal(0, catalog.BatchCalls);
        Assert.Null(promotions.AddedPromotion);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    private static CreatePromotionInput CreateInput(
        IReadOnlyCollection<Guid> gameIds) =>
        new(
            "FIAP Week",
            10m,
            PromotionsTestData.NowUtc.AddHours(1),
            PromotionsTestData.NowUtc.AddHours(2),
            gameIds);

    private static CreatePromotionService CreateService(
        FakePromotionRepository promotions,
        SpyPromotionsUnitOfWork unitOfWork,
        StubCatalogModule catalog) =>
        new(
            promotions,
            unitOfWork,
            catalog,
            new FixedTimeProvider(PromotionsTestData.NowUtc),
            NullLogger<CreatePromotionService>.Instance);
}
