using FiapCloudGames.Promotions.Application.Features.Pricing.GetPromotionalPrice;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Promotions.UnitTests.Application;

public sealed class GetPromotionalPriceServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithoutActivePromotion_ShouldReturnBasePrice()
    {
        var gameId = Guid.NewGuid();
        var promotions = new FakePromotionRepository();
        var service = CreateService(promotions);

        var result = await service.ExecuteAsync(
            gameId,
            100m,
            CancellationToken.None);

        Assert.Equal(100m, result.BasePrice);
        Assert.Equal(100m, result.FinalPrice);
        Assert.Equal(0m, result.DiscountPercent);
        Assert.Null(result.PromotionId);
        Assert.Equal(gameId, promotions.RequestedGameId);
        Assert.Equal(PromotionsTestData.NowUtc, promotions.RequestedInstant);
    }

    [Fact]
    public async Task ExecuteAsync_WithActivePromotion_ShouldReturnDiscountedPrice()
    {
        var gameId = Guid.NewGuid();
        var promotion = PromotionsTestData.CreatePromotion(
            gameId,
            discountPercent: 25m);
        var promotions = new FakePromotionRepository
        {
            ActivePromotion = promotion
        };
        var service = CreateService(promotions);

        var result = await service.ExecuteAsync(
            gameId,
            100m,
            CancellationToken.None);

        Assert.Equal(100m, result.BasePrice);
        Assert.Equal(75m, result.FinalPrice);
        Assert.Equal(25m, result.DiscountPercent);
        Assert.Equal(promotion.Id, result.PromotionId);
    }

    private static GetPromotionalPriceService CreateService(
        FakePromotionRepository promotions) =>
        new(
            promotions,
            new FixedTimeProvider(PromotionsTestData.NowUtc),
            NullLogger<GetPromotionalPriceService>.Instance);
}
