using FiapCloudGames.Library.Application.Features.UserLibrary.GetLibrary;
using FiapCloudGames.Library.Contracts;

namespace FiapCloudGames.Library.Application.Integrations;

internal sealed class LibraryModule(GetLibraryService service)
    : ILibraryModule
{
    public async Task<UserLibrarySnapshot> GetLibraryAsync(
        GetUserLibraryQuery query,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            query.UserId,
            cancellationToken);

        return new UserLibrarySnapshot(
            result.UserId,
            result.Games
                .Select(item => new LibraryItemSnapshot(
                    item.Id,
                    item.GameId,
                    item.GameTitle,
                    item.PricePaid,
                    item.PromotionId,
                    item.AcquiredAtUtc))
                .ToArray());
    }
}
