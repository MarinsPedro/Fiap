using FiapCloudGames.Library.Domain.Entities;

namespace FiapCloudGames.Library.UnitTests;

public sealed class GameLibraryTests
{
    [Fact]
    public void AddGameShouldPreventDuplicates()
    {
        var library = GameLibrary.Create(Guid.NewGuid());
        var gameId = Guid.NewGuid();
        library.AddGame(gameId, 49.90m, null, DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(() =>
            library.AddGame(gameId, 49.90m, null, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AddGameShouldKeepPurchaseSnapshot()
    {
        var library = GameLibrary.Create(Guid.NewGuid());
        var acquiredAt = DateTimeOffset.UtcNow;
        var promotionId = Guid.NewGuid();

        var item = library.AddGame(Guid.NewGuid(), 79.995m, promotionId, acquiredAt);

        Assert.Equal(80.00m, item.PricePaid);
        Assert.Equal(promotionId, item.PromotionId);
        Assert.Equal(acquiredAt, item.AcquiredAtUtc);
    }
}
