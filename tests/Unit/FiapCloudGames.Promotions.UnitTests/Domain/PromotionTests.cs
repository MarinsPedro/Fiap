using FiapCloudGames.Domain.Common;
using FiapCloudGames.Promotions.Domain.Entities;

namespace FiapCloudGames.Promotions.UnitTests.Domain;

public sealed class PromotionTests
{
    private static readonly DateTimeOffset NowUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_ShouldNormalizeAndCreatePromotion()
    {
        var gameId = Guid.NewGuid();

        var promotion = Promotion.Create(
            "  FIAP Week  ",
            25m,
            NowUtc,
            NowUtc.AddHours(1),
            [gameId],
            NowUtc.AddHours(-1));

        Assert.NotEqual(Guid.Empty, promotion.Id);
        Assert.Equal("FIAP Week", promotion.Name);
        Assert.Equal(25m, promotion.DiscountPercent.Value);
        Assert.Equal(NowUtc, promotion.StartsAtUtc);
        Assert.Equal(NowUtc.AddHours(1), promotion.EndsAtUtc);
        Assert.Equal(NowUtc.AddHours(-1), promotion.CreatedAtUtc);
        Assert.Null(promotion.EndedAtUtc);
        Assert.True(promotion.Includes(gameId));
        Assert.Single(promotion.Games);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(120)]
    public void Create_WithNameAtValidBoundary_ShouldCreatePromotion(int length)
    {
        var promotion = CreatePromotion(name: new string('p', length));

        Assert.Equal(length, promotion.Name.Length);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(121)]
    public void Create_WithNameOutsideBoundary_ShouldThrowBusinessRule(
        int length)
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => CreatePromotion(name: new string('p', length)));

        Assert.Equal(
            "O nome da promoção deve ter entre 2 e 120 caracteres.",
            exception.Message);
    }

    [Fact]
    public void Create_WhenEndEqualsStart_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            Promotion.Create(
                "FIAP Week",
                10m,
                NowUtc,
                NowUtc,
                [Guid.NewGuid()],
                NowUtc.AddHours(-1)));

        Assert.Equal(
            "O fim da promoção deve ser posterior ao início.",
            exception.Message);
    }

    [Fact]
    public void Create_WhenEndPrecedesStart_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            Promotion.Create(
                "FIAP Week",
                10m,
                NowUtc,
                NowUtc.AddTicks(-1),
                [Guid.NewGuid()],
                NowUtc.AddHours(-1)));

        Assert.Equal(
            "O fim da promoção deve ser posterior ao início.",
            exception.Message);
    }

    [Fact]
    public void Create_WithoutGames_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => CreatePromotion(gameIds: []));

        Assert.Equal(
            "A promoção deve possuir pelo menos um jogo.",
            exception.Message);
    }

    [Fact]
    public void Create_WithNullGames_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            Promotion.Create(
                "FIAP Week",
                25m,
                NowUtc,
                NowUtc.AddHours(1),
                null!,
                NowUtc.AddHours(-1)));

        Assert.Equal(
            "A promoção deve possuir pelo menos um jogo.",
            exception.Message);
    }

    [Fact]
    public void Create_WithEmptyGameId_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => CreatePromotion(gameIds: [Guid.Empty]));

        Assert.Equal(
            "Todos os identificadores de jogo devem ser válidos.",
            exception.Message);
    }

    [Fact]
    public void Create_WithDuplicateGameIds_ShouldKeepDistinctGames()
    {
        var gameId = Guid.NewGuid();

        var promotion = CreatePromotion(gameIds: [gameId, gameId]);

        var promotionGame = Assert.Single(promotion.Games);
        Assert.Equal(gameId, promotionGame.GameId);
    }

    [Fact]
    public void Create_WithDefaultStartDate_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            Promotion.Create(
                "FIAP Week",
                10m,
                default,
                NowUtc.AddHours(1),
                [Guid.NewGuid()],
                NowUtc.AddHours(-1)));

        Assert.Equal("O início da promoção deve estar em UTC.", exception.Message);
    }

    [Fact]
    public void Create_WithNonUtcEndDate_ShouldThrowBusinessRule()
    {
        var nonUtc = NowUtc.AddHours(1).ToOffset(TimeSpan.FromHours(-3));

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            Promotion.Create(
                "FIAP Week",
                10m,
                NowUtc,
                nonUtc,
                [Guid.NewGuid()],
                NowUtc.AddHours(-1)));

        Assert.Equal("O fim da promoção deve estar em UTC.", exception.Message);
    }

    [Fact]
    public void Create_WithDefaultCreationDate_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            Promotion.Create(
                "FIAP Week",
                10m,
                NowUtc,
                NowUtc.AddHours(1),
                [Guid.NewGuid()],
                default));

        Assert.Equal(
            "A data de criação da promoção deve estar em UTC.",
            exception.Message);
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(30, true)]
    [InlineData(60, false)]
    [InlineData(61, false)]
    public void IsActiveAt_AroundPeriodBoundaries_ShouldReturnExpectedResult(
        int minutesFromStart,
        bool expected)
    {
        var promotion = CreatePromotion();

        var result = promotion.IsActiveAt(
            NowUtc.AddMinutes(minutesFromStart));

        Assert.Equal(expected, result);
    }

    [Fact]
    public void End_WithValidInstant_ShouldEndAndDeactivatePromotion()
    {
        var promotion = CreatePromotion();
        var endedAtUtc = NowUtc.AddMinutes(30);

        promotion.End(endedAtUtc);

        Assert.Equal(endedAtUtc, promotion.EndedAtUtc);
        Assert.False(promotion.IsActiveAt(endedAtUtc));
    }

    [Fact]
    public void End_WhenAlreadyEnded_ShouldKeepFirstEndInstant()
    {
        var promotion = CreatePromotion();
        var firstEnd = NowUtc.AddMinutes(10);
        promotion.End(firstEnd);

        promotion.End(NowUtc.AddMinutes(20));

        Assert.Equal(firstEnd, promotion.EndedAtUtc);
    }

    [Fact]
    public void End_BeforeCreation_ShouldThrowBusinessRule()
    {
        var promotion = CreatePromotion();

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => promotion.End(NowUtc.AddHours(-2)));

        Assert.Equal(
            "A promoção não pode terminar antes de ser criada.",
            exception.Message);
    }

    [Fact]
    public void End_WithDefaultDate_ShouldThrowBusinessRule()
    {
        var promotion = CreatePromotion();

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => promotion.End(default));

        Assert.Equal("A data de encerramento deve estar em UTC.", exception.Message);
    }

    [Fact]
    public void End_WithNonUtcDate_ShouldThrowBusinessRule()
    {
        var promotion = CreatePromotion();
        var nonUtc = NowUtc.ToOffset(TimeSpan.FromHours(-3));

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => promotion.End(nonUtc));

        Assert.Equal("A data de encerramento deve estar em UTC.", exception.Message);
    }

    private static Promotion CreatePromotion(
        string name = "FIAP Week",
        IEnumerable<Guid>? gameIds = null) =>
        Promotion.Create(
            name,
            25m,
            NowUtc,
            NowUtc.AddHours(1),
            gameIds ?? [Guid.NewGuid()],
            NowUtc.AddHours(-1));
}
