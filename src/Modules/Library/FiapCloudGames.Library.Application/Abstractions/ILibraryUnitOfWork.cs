namespace FiapCloudGames.Library.Application.Abstractions;

public interface ILibraryUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
