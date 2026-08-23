using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Promotions.Application.Features.Promotions.GetPromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.ListActivePromotions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Promotions.UnitTests.Application;

public sealed class PromotionQueryServicesTests
{
    [Fact]
    public async Task GetPromotion_WithExistingPromotion_ShouldReturnMappedResult()
    {
        var promotion = PromotionsTestData.CreatePromotion();
        var service = new GetPromotionService(
            new FakePromotionRepository { Promotion = promotion },
            NullLogger<GetPromotionService>.Instance);

        var result = await service.ExecuteAsync(
            promotion.Id,
            CancellationToken.None);

        Assert.Equal(promotion.Id, result.Id);
        Assert.Equal(promotion.Name, result.Name);
        Assert.Equal(promotion.DiscountPercent.Value, result.DiscountPercent);
        Assert.Equal(
            promotion.Games.Select(game => game.GameId),
            result.GameIds);
    }

    [Fact]
    public async Task GetPromotion_WhenPromotionDoesNotExist_ShouldThrowNotFound()
    {
        var service = new GetPromotionService(
            new FakePromotionRepository(),
            NullLogger<GetPromotionService>.Instance);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Equal("Promoção não encontrada.", exception.Message);
    }

    [Fact]
    public async Task ListActivePromotions_ShouldUseCurrentTimeAndReturnMappedResults()
    {
        var promotion = PromotionsTestData.CreatePromotion();
        var promotions = new FakePromotionRepository
        {
            ActivePromotions = [promotion]
        };
        var service = new ListActivePromotionsService(
            promotions,
            new FixedTimeProvider(PromotionsTestData.NowUtc),
            NullLogger<ListActivePromotionsService>.Instance);

        var results = await service.ExecuteAsync(CancellationToken.None);

        var result = Assert.Single(results);
        Assert.Equal(promotion.Id, result.Id);
        Assert.Equal(PromotionsTestData.NowUtc, promotions.RequestedInstant);
    }
}
