using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Application.Features.Games.DeactivateGame;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Catalog.UnitTests.Application;

public sealed class DeactivateGameServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithActiveGame_ShouldDeactivateAndPersist()
    {
        var game = CatalogTestData.CreateGame();
        var unitOfWork = new SpyCatalogUnitOfWork();
        var service = CreateService(
            new FakeGameRepository { Game = game },
            unitOfWork);

        await service.ExecuteAsync(game.Id, CancellationToken.None);

        Assert.False(game.IsActive);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGameDoesNotExist_ShouldThrowNotFoundWithoutPersisting()
    {
        var unitOfWork = new SpyCatalogUnitOfWork();
        var service = CreateService(new FakeGameRepository(), unitOfWork);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Equal("Jogo não encontrado.", exception.Message);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGameIsInactive_ShouldThrowConflictWithoutPersisting()
    {
        var game = CatalogTestData.CreateGame();
        game.Deactivate();
        var unitOfWork = new SpyCatalogUnitOfWork();
        var service = CreateService(
            new FakeGameRepository { Game = game },
            unitOfWork);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(game.Id, CancellationToken.None));

        Assert.Equal(AppErrorCategory.Conflict, exception.Category);
        Assert.Equal("Jogo já está desativado.", exception.Message);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    private static DeactivateGameService CreateService(
        FakeGameRepository games,
        SpyCatalogUnitOfWork unitOfWork) =>
        new(
            games,
            unitOfWork,
            NullLogger<DeactivateGameService>.Instance);
}
