using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Catalog.Infrastructure.Persistence;

internal sealed class GameRepository(CatalogDbContext dbContext) : IGameRepository
{
    public async Task AddAsync(Game game, CancellationToken cancellationToken) =>
        await dbContext.Games.AddAsync(game, cancellationToken);

    public Task<Game?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Games.SingleOrDefaultAsync(game => game.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Game>> ListByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await dbContext.Games
            .AsNoTracking()
            .Where(game => ids.Contains(game.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Game>> ListAsync(bool onlyActive, CancellationToken cancellationToken)
    {
        var query = dbContext.Games.AsNoTracking();
        if (onlyActive)
        {
            query = query.Where(game => game.IsActive);
        }

        return await query.OrderBy(game => game.Title).ToListAsync(cancellationToken);
    }
}
