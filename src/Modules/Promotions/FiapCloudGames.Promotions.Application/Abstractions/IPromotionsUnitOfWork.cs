namespace FiapCloudGames.Promotions.Application.Abstractions;

public interface IPromotionsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
