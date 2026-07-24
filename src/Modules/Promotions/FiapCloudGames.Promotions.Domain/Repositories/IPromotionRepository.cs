using FiapCloudGames.Promotions.Domain.Entities;

namespace FiapCloudGames.Promotions.Domain.Repositories;

public interface IPromotionRepository
{
    Task AddAsync(Promotion promotion, CancellationToken cancellationToken);
    Task<Promotion?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<Promotion?> GetActiveForGameAsync(Guid gameId, DateTimeOffset instant, CancellationToken cancellationToken);
    Task<IReadOnlyList<Promotion>> ListActiveAsync(DateTimeOffset instant, CancellationToken cancellationToken);
}
