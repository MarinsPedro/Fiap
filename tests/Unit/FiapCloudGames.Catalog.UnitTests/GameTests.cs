using FiapCloudGames.Catalog.Domain.Entities;

namespace FiapCloudGames.Catalog.UnitTests;

public sealed class GameTests
{
    [Fact]
    public void CreateShouldNormalizeMoney()
    {
        var game = Game.Create("Cloud Quest", "Aventura", "RPG", 99.999m);

        Assert.Equal(100.00m, game.BasePrice);
        Assert.True(game.IsActive);
    }

    [Fact]
    public void CreateShouldRejectNegativePrice()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Game.Create("Cloud Quest", "Aventura", "RPG", -0.01m));
    }
}
