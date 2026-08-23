using FiapCloudGames.Catalog.Domain.Entities;

namespace FiapCloudGames.Catalog.Domain.Repositories;

public interface IGameRepository
{
    Task AddAsync(Game game, CancellationToken cancellationToken);
    Task<Game?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Game>> ListByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Game>> ListAsync(bool onlyActive, CancellationToken cancellationToken);
}
