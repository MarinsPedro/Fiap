using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Application.Features.Games.UpdateGame;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Catalog.UnitTests.Application;

public sealed class UpdateGameServiceTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExecuteAsync_WithExistingGame_ShouldUpdateAndPreserveLifecycle(
        bool isActive)
    {
        var game = CatalogTestData.CreateGame();
        if (!isActive)
        {
            game.Deactivate();
        }

        var unitOfWork = new SpyCatalogUnitOfWork();
        var service = CreateService(
            new FakeGameRepository { Game = game },
            unitOfWork);

        var result = await service.ExecuteAsync(
            game.Id,
            new UpdateGameInput(
                "Cloud Quest Deluxe",
                "Edição atualizada",
                "RPG",
                20m),
            CancellationToken.None);

        Assert.Equal("Cloud Quest Deluxe", game.Title);
        Assert.Equal(20m, game.BasePrice.Amount);
        Assert.Equal(isActive, game.IsActive);
        Assert.Equal(isActive, result.IsActive);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenGameDoesNotExist_ShouldThrowNotFoundWithoutPersisting()
    {
        var unitOfWork = new SpyCatalogUnitOfWork();
        var service = CreateService(new FakeGameRepository(), unitOfWork);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                Guid.NewGuid(),
                new UpdateGameInput("Cloud Quest", "Descrição", "RPG", 20m),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Equal("Jogo não encontrado.", exception.Message);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    private static UpdateGameService CreateService(
        FakeGameRepository games,
        SpyCatalogUnitOfWork unitOfWork) =>
        new(
            games,
            unitOfWork,
            NullLogger<UpdateGameService>.Instance);
}
