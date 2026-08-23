using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Domain.Common;

namespace FiapCloudGames.Catalog.UnitTests.Domain;

public sealed class GameTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_ShouldNormalizeAndCreateActiveGame()
    {
        var game = Game.Create(
            "  Cloud Quest  ",
            "  Aventura  ",
            "  RPG  ",
            99.995m,
            CreatedAtUtc);

        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal("Cloud Quest", game.Title);
        Assert.Equal("Aventura", game.Description);
        Assert.Equal("RPG", game.Category);
        Assert.Equal(100.00m, game.BasePrice.Amount);
        Assert.True(game.IsActive);
        Assert.Equal(CreatedAtUtc, game.CreatedAtUtc);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(160)]
    public void Create_WithTitleAtValidBoundary_ShouldCreateGame(int length)
    {
        var game = CreateGame(title: new string('t', length));

        Assert.Equal(length, game.Title.Length);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(161)]
    public void Create_WithTitleOutsideBoundary_ShouldThrowBusinessRule(
        int length)
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => CreateGame(title: new string('t', length)));

        Assert.Equal(
            "O título deve ter entre 2 e 160 caracteres.",
            exception.Message);
    }

    [Fact]
    public void Create_WithDescriptionAtMaximumLength_ShouldCreateGame()
    {
        var game = CreateGame(description: new string('d', 4000));

        Assert.Equal(4000, game.Description.Length);
    }

    [Fact]
    public void Create_WithDescriptionAboveMaximumLength_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => CreateGame(description: new string('d', 4001)));

        Assert.Equal(
            "A descrição deve ter no máximo 4000 caracteres.",
            exception.Message);
    }

    [Fact]
    public void Create_WithoutDescription_ShouldCreateGameWithEmptyDescription()
    {
        var game = CreateGame(description: " ");

        Assert.Equal(string.Empty, game.Description);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(80)]
    public void Create_WithCategoryAtValidBoundary_ShouldCreateGame(int length)
    {
        var game = CreateGame(category: new string('c', length));

        Assert.Equal(length, game.Category.Length);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(81)]
    public void Create_WithCategoryOutsideBoundary_ShouldThrowBusinessRule(
        int length)
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => CreateGame(category: new string('c', length)));

        Assert.Equal(
            "A categoria é obrigatória e deve ter no máximo 80 caracteres.",
            exception.Message);
    }

    [Fact]
    public void Create_WithDefaultCreationDate_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => Game.Create(
                "Cloud Quest",
                "Aventura",
                "RPG",
                99.90m,
                default));

        Assert.Equal(
            "A data de criação do jogo deve estar em UTC.",
            exception.Message);
    }

    [Fact]
    public void Create_WithNonUtcCreationDate_ShouldThrowBusinessRule()
    {
        var nonUtc = CreatedAtUtc.ToOffset(TimeSpan.FromHours(-3));

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => CreateGame(createdAtUtc: nonUtc));

        Assert.Equal(
            "A data de criação do jogo deve estar em UTC.",
            exception.Message);
    }

    [Fact]
    public void ChangeDetails_WithValidData_ShouldUpdateGame()
    {
        var game = CreateGame();

        game.ChangeDetails(
            "  Cloud Quest Deluxe  ",
            "  Edição atualizada  ",
            "  Aventura  ",
            149.90m);

        Assert.Equal("Cloud Quest Deluxe", game.Title);
        Assert.Equal("Edição atualizada", game.Description);
        Assert.Equal("Aventura", game.Category);
        Assert.Equal(149.90m, game.BasePrice.Amount);
        Assert.True(game.IsActive);
    }

    [Fact]
    public void Deactivate_WhenGameIsActive_ShouldDeactivate()
    {
        var game = CreateGame();

        game.Deactivate();

        Assert.False(game.IsActive);
    }

    [Fact]
    public void Activate_WhenGameIsInactive_ShouldActivate()
    {
        var game = CreateGame();
        game.Deactivate();

        game.Activate();

        Assert.True(game.IsActive);
    }

    private static Game CreateGame(
        string title = "Cloud Quest",
        string description = "Aventura",
        string category = "RPG",
        DateTimeOffset? createdAtUtc = null) =>
        Game.Create(
            title,
            description,
            category,
            99.90m,
            createdAtUtc ?? CreatedAtUtc);
}
