using FiapCloudGames.Library.Domain.Entities;
using FiapCloudGames.Library.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Library.Infrastructure.Persistence;

internal sealed class GameLibraryRepository(LibraryDbContext dbContext) : IGameLibraryRepository
{
    public async Task AddAsync(GameLibrary library, CancellationToken cancellationToken) =>
        await dbContext.Libraries.AddAsync(library, cancellationToken);

    public Task<GameLibrary?> GetByUserAsync(
        Guid userId,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Libraries.Include(library => library.Games);
        return trackChanges
            ? query.SingleOrDefaultAsync(library => library.UserId == userId, cancellationToken)
            : query.AsNoTracking().SingleOrDefaultAsync(library => library.UserId == userId, cancellationToken);
    }
}
