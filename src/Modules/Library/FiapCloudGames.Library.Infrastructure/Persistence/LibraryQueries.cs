using FiapCloudGames.Library.Application.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Library.Infrastructure.Persistence;

internal sealed class LibraryQueries(
    LibraryDbContext dbContext) : ILibraryQueries
{
    public async Task<IReadOnlyList<LibraryGameReadModel>>
        ListGamesAsync(
            Guid userId,
            CancellationToken cancellationToken)
    {
        var library = await dbContext.Libraries
            .AsNoTracking()
            .Include(item => item.Games)
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        return library is null
            ? []
            : library.Games
                .OrderByDescending(item => item.AcquiredAtUtc)
                .Select(item => new LibraryGameReadModel(
                    item.Id,
                    item.GameId,
                    item.PricePaid.Amount,
                    item.PromotionId,
                    item.AcquiredAtUtc))
                .ToArray();
    }
}
