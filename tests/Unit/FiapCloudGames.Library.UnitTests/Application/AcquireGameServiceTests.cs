using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Identity.Contracts;
using FiapCloudGames.Library.Application.Features.UserLibrary.AcquireGame;
using FiapCloudGames.Library.Domain.Entities;
using FiapCloudGames.Promotions.Contracts;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Library.UnitTests.Application;

public sealed class AcquireGameServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithoutExistingLibrary_ShouldCreateLibraryAndPersistBasePrice()
    {
        var scenario = CreateScenario();

        var result = await scenario.Service.ExecuteAsync(
            scenario.GameId,
            CancellationToken.None);

        var library = Assert.IsType<GameLibrary>(
            scenario.Libraries.AddedLibrary);
        var item = Assert.Single(library.Games);
        Assert.Equal(scenario.UserId, library.UserId);
        Assert.Equal(LibraryTestData.NowUtc, library.CreatedAtUtc);
        Assert.Equal(scenario.GameId, item.GameId);
        Assert.Equal(100m, item.PricePaid.Amount);
        Assert.Null(item.PromotionId);
        Assert.Equal(LibraryTestData.NowUtc, item.AcquiredAtUtc);
        Assert.Equal(item.Id, result.Id);
        Assert.Equal("Cloud Quest", result.GameTitle);
        Assert.Equal(1, scenario.Libraries.AddCalls);
        Assert.Equal(1, scenario.UnitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingLibraryAndPromotion_ShouldPersistQuotedPrice()
    {
        var promotionId = Guid.NewGuid();
        var scenario = CreateScenario();
        var library = GameLibrary.Create(
            scenario.UserId,
            LibraryTestData.NowUtc.AddDays(-1));
        scenario.Libraries.Library = library;
        scenario.Promotions.Result = new PriceQuoteSnapshot(
            100m,
            75m,
            25m,
            promotionId);

        var result = await scenario.Service.ExecuteAsync(
            scenario.GameId,
            CancellationToken.None);

        var item = Assert.Single(library.Games);
        Assert.Equal(75m, item.PricePaid.Amount);
        Assert.Equal(promotionId, item.PromotionId);
        Assert.Equal(75m, result.PricePaid);
        Assert.Equal(0, scenario.Libraries.AddCalls);
        Assert.Equal(scenario.GameId, scenario.Promotions.RequestedQuery?.GameId);
        Assert.Equal(100m, scenario.Promotions.RequestedQuery?.BasePrice);
        Assert.Equal(1, scenario.UnitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutAuthenticatedUser_ShouldThrowAuthenticationWithoutPersisting()
    {
        var scenario = CreateScenario(authenticated: false);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            scenario.Service.ExecuteAsync(
                scenario.GameId,
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.Authentication, exception.Category);
        Assert.Equal(0, scenario.Libraries.AddCalls);
        Assert.Equal(0, scenario.UnitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowNotFoundWithoutPersisting()
    {
        var scenario = CreateScenario();
        scenario.Identity.Result = null;

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            scenario.Service.ExecuteAsync(
                scenario.GameId,
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Equal("Usuário não encontrado.", exception.Message);
        Assert.Equal(0, scenario.Libraries.AddCalls);
        Assert.Equal(0, scenario.UnitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsInactive_ShouldThrowBusinessRuleWithoutPersisting()
    {
        var scenario = CreateScenario();
        scenario.Identity.Result = new UserSnapshot(scenario.UserId, false);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            scenario.Service.ExecuteAsync(
                scenario.GameId,
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.BusinessRule, exception.Category);
        Assert.Equal("O usuário está inativo.", exception.Message);
        Assert.Equal(0, scenario.Libraries.AddCalls);
        Assert.Equal(0, scenario.UnitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGameDoesNotExist_ShouldThrowNotFoundWithoutPersisting()
    {
        var scenario = CreateScenario();
        scenario.Catalog.GameResult = null;

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            scenario.Service.ExecuteAsync(
                scenario.GameId,
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Equal("Jogo não encontrado.", exception.Message);
        Assert.Equal(0, scenario.Libraries.AddCalls);
        Assert.Equal(0, scenario.UnitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGameIsInactive_ShouldThrowBusinessRuleWithoutPersisting()
    {
        var scenario = CreateScenario();
        scenario.Catalog.GameResult = new GameSnapshot(
            scenario.GameId,
            "Cloud Quest",
            100m,
            false);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            scenario.Service.ExecuteAsync(
                scenario.GameId,
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.BusinessRule, exception.Category);
        Assert.Equal("O jogo está inativo.", exception.Message);
        Assert.Equal(0, scenario.Libraries.AddCalls);
        Assert.Equal(0, scenario.UnitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGameIsAlreadyAcquired_ShouldThrowConflictWithoutPersisting()
    {
        var scenario = CreateScenario();
        var library = GameLibrary.Create(
            scenario.UserId,
            LibraryTestData.NowUtc.AddDays(-1));
        library.AcquireGame(
            scenario.GameId,
            90m,
            null,
            LibraryTestData.NowUtc.AddHours(-1));
        scenario.Libraries.Library = library;

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            scenario.Service.ExecuteAsync(
                scenario.GameId,
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.Conflict, exception.Category);
        Assert.Equal(
            "O jogo já pertence à biblioteca do usuário.",
            exception.Message);
        Assert.Single(library.Games);
        Assert.Equal(0, scenario.Promotions.Calls);
        Assert.Equal(0, scenario.UnitOfWork.SaveChangesCount);
    }

    private static AcquireGameScenario CreateScenario(
        bool authenticated = true)
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var libraries = new FakeGameLibraryRepository();
        var unitOfWork = new SpyLibraryUnitOfWork();
        var identity = new StubIdentityModule
        {
            Result = new UserSnapshot(userId, true)
        };
        var catalog = new StubCatalogModule
        {
            GameResult = new GameSnapshot(
                gameId,
                "Cloud Quest",
                100m,
                true)
        };
        var promotions = new StubPromotionsModule();
        var service = new AcquireGameService(
            new StubCurrentUserContext(authenticated ? userId : null),
            libraries,
            unitOfWork,
            identity,
            catalog,
            promotions,
            new FixedTimeProvider(LibraryTestData.NowUtc),
            NullLogger<AcquireGameService>.Instance);

        return new AcquireGameScenario(
            userId,
            gameId,
            libraries,
            unitOfWork,
            identity,
            catalog,
            promotions,
            service);
    }

    private sealed record AcquireGameScenario(
        Guid UserId,
        Guid GameId,
        FakeGameLibraryRepository Libraries,
        SpyLibraryUnitOfWork UnitOfWork,
        StubIdentityModule Identity,
        StubCatalogModule Catalog,
        StubPromotionsModule Promotions,
        AcquireGameService Service);
}
