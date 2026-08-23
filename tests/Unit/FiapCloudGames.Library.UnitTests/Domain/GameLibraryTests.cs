using FiapCloudGames.Domain.Common;
using FiapCloudGames.Library.Domain.Entities;

namespace FiapCloudGames.Library.UnitTests.Domain;

public sealed class GameLibraryTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_ShouldCreateEmptyLibrary()
    {
        var userId = Guid.NewGuid();

        var library = GameLibrary.Create(userId, CreatedAtUtc);

        Assert.NotEqual(Guid.Empty, library.Id);
        Assert.Equal(userId, library.UserId);
        Assert.Equal(CreatedAtUtc, library.CreatedAtUtc);
        Assert.Empty(library.Games);
    }

    [Fact]
    public void Create_WithoutUser_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => GameLibrary.Create(Guid.Empty, CreatedAtUtc));

        Assert.Equal("O usuário é obrigatório.", exception.Message);
    }

    [Fact]
    public void Create_WithDefaultCreationDate_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => GameLibrary.Create(Guid.NewGuid(), default));

        Assert.Equal(
            "A data de criação da biblioteca deve estar em UTC.",
            exception.Message);
    }

    [Fact]
    public void Create_WithNonUtcCreationDate_ShouldThrowBusinessRule()
    {
        var nonUtc = CreatedAtUtc.ToOffset(TimeSpan.FromHours(-3));

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => GameLibrary.Create(Guid.NewGuid(), nonUtc));

        Assert.Equal(
            "A data de criação da biblioteca deve estar em UTC.",
            exception.Message);
    }

    [Fact]
    public void AcquireGame_WithValidData_ShouldKeepPurchaseSnapshot()
    {
        var library = CreateLibrary();
        var gameId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();
        var acquiredAtUtc = CreatedAtUtc.AddHours(1);

        var item = library.AcquireGame(
            gameId,
            79.995m,
            promotionId,
            acquiredAtUtc);

        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal(library.Id, item.LibraryId);
        Assert.Equal(gameId, item.GameId);
        Assert.Equal(80.00m, item.PricePaid.Amount);
        Assert.Equal(promotionId, item.PromotionId);
        Assert.Equal(acquiredAtUtc, item.AcquiredAtUtc);
        Assert.True(library.ContainsGame(gameId));
        Assert.Same(item, Assert.Single(library.Games));
    }

    [Fact]
    public void AcquireGame_WhenGameAlreadyExists_ShouldThrowBusinessRule()
    {
        var library = CreateLibrary();
        var gameId = Guid.NewGuid();
        library.AcquireGame(
            gameId,
            49.90m,
            null,
            CreatedAtUtc);

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            library.AcquireGame(
                gameId,
                49.90m,
                null,
                CreatedAtUtc));

        Assert.Equal(
            "O jogo já pertence à biblioteca do usuário.",
            exception.Message);
        Assert.Single(library.Games);
    }

    [Fact]
    public void AcquireGame_WithoutGame_ShouldThrowBusinessRule()
    {
        var library = CreateLibrary();

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            library.AcquireGame(Guid.Empty, 10m, null, CreatedAtUtc));

        Assert.Equal("O jogo é obrigatório.", exception.Message);
        Assert.Empty(library.Games);
    }

    [Fact]
    public void AcquireGame_WithEmptyPromotionId_ShouldThrowBusinessRule()
    {
        var library = CreateLibrary();

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            library.AcquireGame(
                Guid.NewGuid(),
                10m,
                Guid.Empty,
                CreatedAtUtc));

        Assert.Equal("A promoção informada é inválida.", exception.Message);
        Assert.Empty(library.Games);
    }

    [Fact]
    public void AcquireGame_WithNegativePrice_ShouldThrowBusinessRule()
    {
        var library = CreateLibrary();

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            library.AcquireGame(
                Guid.NewGuid(),
                -0.01m,
                null,
                CreatedAtUtc));

        Assert.Equal("O preço pago não pode ser negativo.", exception.Message);
        Assert.Empty(library.Games);
    }

    [Fact]
    public void AcquireGame_AtLibraryCreationTime_ShouldAcquireGame()
    {
        var library = CreateLibrary();

        var item = library.AcquireGame(
            Guid.NewGuid(),
            10m,
            null,
            CreatedAtUtc);

        Assert.Equal(CreatedAtUtc, item.AcquiredAtUtc);
    }

    [Fact]
    public void AcquireGame_BeforeLibraryCreation_ShouldThrowBusinessRule()
    {
        var library = CreateLibrary();

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            library.AcquireGame(
                Guid.NewGuid(),
                10m,
                null,
                CreatedAtUtc.AddTicks(-1)));

        Assert.Equal(
            "A aquisição não pode ocorrer antes da criação da biblioteca.",
            exception.Message);
        Assert.Empty(library.Games);
    }

    [Fact]
    public void AcquireGame_WithDefaultAcquisitionDate_ShouldThrowBusinessRule()
    {
        var library = CreateLibrary();

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            library.AcquireGame(Guid.NewGuid(), 10m, null, default));

        Assert.Equal("A data de aquisição deve estar em UTC.", exception.Message);
        Assert.Empty(library.Games);
    }

    [Fact]
    public void AcquireGame_WithNonUtcAcquisitionDate_ShouldThrowBusinessRule()
    {
        var library = CreateLibrary();
        var nonUtc = CreatedAtUtc.ToOffset(TimeSpan.FromHours(-3));

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            library.AcquireGame(Guid.NewGuid(), 10m, null, nonUtc));

        Assert.Equal("A data de aquisição deve estar em UTC.", exception.Message);
        Assert.Empty(library.Games);
    }

    private static GameLibrary CreateLibrary() =>
        GameLibrary.Create(Guid.NewGuid(), CreatedAtUtc);
}
