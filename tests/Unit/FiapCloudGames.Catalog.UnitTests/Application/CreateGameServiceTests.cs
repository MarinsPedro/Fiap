using FiapCloudGames.Catalog.Application.Features.Games.CreateGame;
using FiapCloudGames.Domain.Common;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Catalog.UnitTests.Application;

public sealed class CreateGameServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldCreateAndPersistGame()
    {
        var games = new FakeGameRepository();
        var unitOfWork = new SpyCatalogUnitOfWork();
        var service = CreateService(games, unitOfWork);

        var result = await service.ExecuteAsync(
            new CreateGameInput(
                "  Cloud Quest  ",
                "  Aventura  ",
                "  RPG  ",
                99.995m),
            CancellationToken.None);

        var addedGame = Assert.IsType<Catalog.Domain.Entities.Game>(
            games.AddedGame);
        Assert.Equal(1, games.AddCalls);
        Assert.Equal("Cloud Quest", addedGame.Title);
        Assert.Equal(100.00m, addedGame.BasePrice.Amount);
        Assert.Equal(CatalogTestData.NowUtc, addedGame.CreatedAtUtc);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
        Assert.Equal(addedGame.Id, result.Id);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidInput_ShouldNotPersist()
    {
        var games = new FakeGameRepository();
        var unitOfWork = new SpyCatalogUnitOfWork();
        var service = CreateService(games, unitOfWork);

        await Assert.ThrowsAsync<DomainRuleViolationException>(() =>
            service.ExecuteAsync(
                new CreateGameInput("A", "Descrição", "RPG", 10m),
                CancellationToken.None));

        Assert.Null(games.AddedGame);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    private static CreateGameService CreateService(
        FakeGameRepository games,
        SpyCatalogUnitOfWork unitOfWork) =>
        new(
            games,
            unitOfWork,
            new FixedTimeProvider(CatalogTestData.NowUtc),
            NullLogger<CreateGameService>.Instance);
}
