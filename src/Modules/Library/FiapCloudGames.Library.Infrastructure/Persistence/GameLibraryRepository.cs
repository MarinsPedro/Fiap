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
        CancellationToken cancellationToken)
        => dbContext.Libraries
            .Include(library => library.Games)
            .SingleOrDefaultAsync(
                library => library.UserId == userId,
                cancellationToken);
}
