using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Domain.Common;

namespace FiapCloudGames.Catalog.UnitTests;

public sealed class GameTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void CreateShouldNormalizeDetailsAndMoney()
    {
        var game = Game.Create(
            "  Cloud Quest  ",
            "  Aventura  ",
            "  RPG  ",
            99.999m,
            CreatedAtUtc);

        Assert.Equal("Cloud Quest", game.Title);
        Assert.Equal("Aventura", game.Description);
        Assert.Equal("RPG", game.Category);
        Assert.Equal(100.00m, game.BasePrice.Amount);
        Assert.Equal(CreatedAtUtc, game.CreatedAtUtc);
        Assert.True(game.IsActive);
    }

    [Fact]
    public void CreateShouldRejectNegativePrice()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            Game.Create(
                "Cloud Quest",
                "Aventura",
                "RPG",
                -0.01m,
                CreatedAtUtc));

        Assert.Equal(
            "O preço base não pode ser negativo.",
            exception.Message);
    }

    [Fact]
    public void ChangeDetailsShouldEnforceDatabaseLengthLimits()
    {
        var game = Game.Create(
            "Cloud Quest",
            "Aventura",
            "RPG",
            10m,
            CreatedAtUtc);

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            game.ChangeDetails(
                "Cloud Quest",
                new string('x', 4001),
                "RPG",
                10m));

        Assert.Equal(
            "A descrição deve ter no máximo 4000 caracteres.",
            exception.Message);
    }

    [Fact]
    public void ActivateAndDeactivateShouldExpressLifecycle()
    {
        var game = Game.Create(
            "Cloud Quest",
            "Aventura",
            "RPG",
            10m,
            CreatedAtUtc);

        game.Deactivate();
        Assert.False(game.IsActive);

        game.Activate();
        Assert.True(game.IsActive);
    }
}
