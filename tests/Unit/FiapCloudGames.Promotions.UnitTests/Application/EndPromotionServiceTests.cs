using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Promotions.Application.Features.Promotions.EndPromotion;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Promotions.UnitTests.Application;

public sealed class EndPromotionServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingPromotion_ShouldEndAndPersist()
    {
        var promotion = PromotionsTestData.CreatePromotion();
        var unitOfWork = new SpyPromotionsUnitOfWork();
        var service = CreateService(
            new FakePromotionRepository { Promotion = promotion },
            unitOfWork);

        await service.ExecuteAsync(promotion.Id, CancellationToken.None);

        Assert.Equal(PromotionsTestData.NowUtc, promotion.EndedAtUtc);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPromotionDoesNotExist_ShouldThrowNotFoundWithoutPersisting()
    {
        var unitOfWork = new SpyPromotionsUnitOfWork();
        var service = CreateService(
            new FakePromotionRepository(),
            unitOfWork);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Equal("Promoção não encontrada.", exception.Message);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    private static EndPromotionService CreateService(
        FakePromotionRepository promotions,
        SpyPromotionsUnitOfWork unitOfWork) =>
        new(
            promotions,
            unitOfWork,
            new FixedTimeProvider(PromotionsTestData.NowUtc),
            NullLogger<EndPromotionService>.Instance);
}
