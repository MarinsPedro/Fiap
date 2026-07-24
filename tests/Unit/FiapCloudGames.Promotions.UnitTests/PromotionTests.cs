using FiapCloudGames.Promotions.Domain.Entities;

namespace FiapCloudGames.Promotions.UnitTests;

public sealed class PromotionTests
{
    [Fact]
    public void ApplyToShouldCalculateDiscount()
    {
        var now = DateTimeOffset.UtcNow;
        var promotion = Promotion.Create("FIAP Week", 25, now.AddHours(-1), now.AddHours(1), [Guid.NewGuid()]);

        Assert.True(promotion.IsActiveAt(now));
        Assert.Equal(75m, promotion.ApplyTo(100m));
    }

    [Fact]
    public void CreateShouldRejectInvalidPeriod()
    {
        var now = DateTimeOffset.UtcNow;

        Assert.Throws<InvalidOperationException>(() =>
            Promotion.Create("FIAP Week", 10, now, now, [Guid.NewGuid()]));
    }
}
