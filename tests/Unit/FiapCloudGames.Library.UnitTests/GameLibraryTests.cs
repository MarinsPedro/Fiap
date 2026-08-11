using FiapCloudGames.Domain.Common;
using FiapCloudGames.Library.Domain.Entities;

namespace FiapCloudGames.Library.UnitTests;

public sealed class GameLibraryTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void AcquireGameShouldPreventDuplicates()
    {
        var library = GameLibrary.Create(
            Guid.NewGuid(),
            CreatedAtUtc);
        var gameId = Guid.NewGuid();
        var acquiredAtUtc = CreatedAtUtc.AddMinutes(1);
        library.AcquireGame(
            gameId,
            49.90m,
            null,
            acquiredAtUtc);

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            library.AcquireGame(
                gameId,
                49.90m,
                null,
                acquiredAtUtc));

        Assert.Equal(
            "O jogo já pertence à biblioteca do usuário.",
            exception.Message);
    }

    [Fact]
    public void AcquireGameShouldKeepPurchaseSnapshot()
    {
        var library = GameLibrary.Create(
            Guid.NewGuid(),
            CreatedAtUtc);
        var acquiredAt = CreatedAtUtc.AddHours(1);
        var promotionId = Guid.NewGuid();

        var item = library.AcquireGame(
            Guid.NewGuid(),
            79.995m,
            promotionId,
            acquiredAt);

        Assert.Equal(80.00m, item.PricePaid.Amount);
        Assert.Equal(promotionId, item.PromotionId);
        Assert.Equal(acquiredAt, item.AcquiredAtUtc);
    }

    [Fact]
    public void AcquireGameShouldNotPrecedeLibraryCreation()
    {
        var library = GameLibrary.Create(
            Guid.NewGuid(),
            CreatedAtUtc);

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            library.AcquireGame(
                Guid.NewGuid(),
                10m,
                null,
                CreatedAtUtc.AddSeconds(-1)));

        Assert.Equal(
            "A aquisição não pode ocorrer antes da criação da biblioteca.",
            exception.Message);
    }
}
