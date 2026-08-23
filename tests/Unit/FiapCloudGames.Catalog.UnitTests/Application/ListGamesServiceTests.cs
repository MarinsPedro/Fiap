using FiapCloudGames.Catalog.Application.Features.Games.ListGames;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Catalog.UnitTests.Application;

public sealed class ListGamesServiceTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExecuteAsync_WithFilter_ShouldReturnMappedGames(
        bool onlyActive)
    {
        var activeGame = CatalogTestData.CreateGame("Ativo");
        var inactiveGame = CatalogTestData.CreateGame("Inativo");
        inactiveGame.Deactivate();
        var games = new FakeGameRepository
        {
            Games = [activeGame, inactiveGame]
        };
        var service = new ListGamesService(
            games,
            NullLogger<ListGamesService>.Instance);

        var results = await service.ExecuteAsync(
            onlyActive,
            CancellationToken.None);

        Assert.Equal(onlyActive, games.RequestedOnlyActive);
        Assert.Collection(
            results,
            result => Assert.True(result.IsActive),
            result => Assert.False(result.IsActive));
    }
}
