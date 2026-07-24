using FiapCloudGames.Promotions.Domain.Entities;
using FiapCloudGames.Promotions.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Promotions.Infrastructure.Persistence;

internal sealed class PromotionRepository(PromotionsDbContext dbContext) : IPromotionRepository
{
    public async Task AddAsync(Promotion promotion, CancellationToken cancellationToken) =>
        await dbContext.Promotions.AddAsync(promotion, cancellationToken);

    public Task<Promotion?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Promotions.Include(promotion => promotion.Games)
            .SingleOrDefaultAsync(promotion => promotion.Id == id, cancellationToken);

    public Task<Promotion?> GetActiveForGameAsync(
        Guid gameId,
        DateTimeOffset instant,
        CancellationToken cancellationToken) =>
        dbContext.Promotions.Include(promotion => promotion.Games)
            .Where(promotion => promotion.EndedAtUtc == null &&
                promotion.StartsAtUtc <= instant &&
                instant < promotion.EndsAtUtc &&
                promotion.Games.Any(item => item.GameId == gameId))
            .OrderByDescending(promotion => promotion.DiscountPercent)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Promotion>> ListActiveAsync(
        DateTimeOffset instant,
        CancellationToken cancellationToken) =>
        await dbContext.Promotions.AsNoTracking()
            .Include(promotion => promotion.Games)
            .Where(promotion => promotion.EndedAtUtc == null &&
                promotion.StartsAtUtc <= instant && instant < promotion.EndsAtUtc)
            .OrderBy(promotion => promotion.EndsAtUtc)
            .ToListAsync(cancellationToken);
}
