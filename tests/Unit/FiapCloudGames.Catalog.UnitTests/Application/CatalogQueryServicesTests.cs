using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Application.Features.Games.FindGame;
using FiapCloudGames.Catalog.Application.Features.Games.FindGames;
using FiapCloudGames.Catalog.Application.Features.Games.GetGame;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Catalog.UnitTests.Application;

public sealed class CatalogQueryServicesTests
{
    [Fact]
    public async Task FindGame_WithExistingGame_ShouldReturnMappedResult()
    {
        var game = CatalogTestData.CreateGame();
        var service = CreateFindService(
            new FakeGameRepository { Game = game });

        var result = await service.ExecuteAsync(
            game.Id,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(game.Id, result.Id);
        Assert.Equal(game.Title, result.Title);
        Assert.Equal(game.BasePrice.Amount, result.BasePrice);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task FindGame_WhenGameDoesNotExist_ShouldReturnNull()
    {
        var service = CreateFindService(new FakeGameRepository());

        var result = await service.ExecuteAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetGame_WhenGameExists_ShouldReturnGame()
    {
        var game = CatalogTestData.CreateGame();
        var service = new GetGameService(
            CreateFindService(new FakeGameRepository { Game = game }));

        var result = await service.ExecuteAsync(
            game.Id,
            CancellationToken.None);

        Assert.Equal(game.Id, result.Id);
    }

    [Fact]
    public async Task GetGame_WhenGameDoesNotExist_ShouldThrowNotFound()
    {
        var service = new GetGameService(
            CreateFindService(new FakeGameRepository()));

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Equal("Jogo não encontrado.", exception.Message);
    }

    [Fact]
    public async Task FindGames_WithIds_ShouldReturnGamesFromSingleBatch()
    {
        var games = new[]
        {
            CatalogTestData.CreateGame("Cloud Quest"),
            CatalogTestData.CreateGame("FIAP Arena")
        };
        var repository = new FakeGameRepository { Games = games };
        var service = new FindGamesService(
            repository,
            NullLogger<FindGamesService>.Instance);
        var ids = games.Select(game => game.Id).ToArray();

        var results = await service.ExecuteAsync(
            ids,
            CancellationToken.None);

        Assert.Equal(2, results.Count);
        Assert.Equal(1, repository.ListByIdsCalls);
        Assert.Equal(ids.Order(), repository.RequestedIds.Order());
    }

    private static FindGameService CreateFindService(
        FakeGameRepository games) =>
        new(games, NullLogger<FindGameService>.Instance);
}
