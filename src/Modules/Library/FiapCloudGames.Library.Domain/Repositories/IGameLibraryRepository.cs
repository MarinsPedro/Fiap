using FiapCloudGames.Library.Domain.Entities;

namespace FiapCloudGames.Library.Domain.Repositories;

public interface IGameLibraryRepository
{
    Task AddAsync(GameLibrary library, CancellationToken cancellationToken);
    Task<GameLibrary?> GetByUserAsync(Guid userId, bool trackChanges, CancellationToken cancellationToken);
}
