using FiapCloudGames.Domain.Common;
using FiapCloudGames.Promotions.Domain.Entities;

namespace FiapCloudGames.Promotions.UnitTests;

public sealed class PromotionTests
{
    private static readonly DateTimeOffset NowUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void ApplyToShouldCalculateDiscount()
    {
        var promotion = Promotion.Create(
            "FIAP Week",
            25,
            NowUtc.AddHours(-1),
            NowUtc.AddHours(1),
            [Guid.NewGuid()],
            NowUtc.AddHours(-2));

        Assert.True(promotion.IsActiveAt(NowUtc));
        Assert.Equal(25m, promotion.DiscountPercent.Value);
        Assert.Equal(75m, promotion.ApplyTo(100m));
    }

    [Fact]
    public void CreateShouldRejectInvalidPeriod()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            Promotion.Create(
                "FIAP Week",
                10,
                NowUtc,
                NowUtc,
                [Guid.NewGuid()],
                NowUtc.AddHours(-1)));

        Assert.Equal(
            "O fim da promoção deve ser posterior ao início.",
            exception.Message);
    }

    [Fact]
    public void CreateShouldRejectPromotionWithoutGames()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            Promotion.Create(
                "FIAP Week",
                10,
                NowUtc,
                NowUtc.AddHours(1),
                [],
                NowUtc.AddHours(-1)));

        Assert.Equal(
            "A promoção deve possuir pelo menos um jogo.",
            exception.Message);
    }

    [Fact]
    public void EndShouldNotPrecedeCreation()
    {
        var promotion = Promotion.Create(
            "FIAP Week",
            10,
            NowUtc,
            NowUtc.AddHours(1),
            [Guid.NewGuid()],
            NowUtc.AddHours(-1));

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            promotion.End(NowUtc.AddHours(-2)));

        Assert.Equal(
            "A promoção não pode terminar antes de ser criada.",
            exception.Message);
    }
}
