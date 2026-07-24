namespace FiapCloudGames.Catalog.Application.Abstractions;

public interface ICatalogUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
