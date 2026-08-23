using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Library.Application.Abstractions.Queries;
using FiapCloudGames.Library.Application.Features.UserLibrary.GetLibrary;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Library.UnitTests;

public sealed class GetLibraryServiceTests
{
    [Fact]
    public async Task GetLibraryShouldLoadCatalogGamesInSingleBatch()
    {
        var userId = Guid.NewGuid();
        var availableGameId = Guid.NewGuid();
        var unavailableGameId = Guid.NewGuid();
        var acquiredAtUtc = new DateTimeOffset(
            2026, 1, 10, 12, 0, 0, TimeSpan.Zero);
        var queries = new LibraryQueries(
        [
            new LibraryGameReadModel(
                Guid.NewGuid(),
                availableGameId,
                10m,
                null,
                acquiredAtUtc),
            new LibraryGameReadModel(
                Guid.NewGuid(),
                unavailableGameId,
                20m,
                null,
                acquiredAtUtc.AddHours(-1))
        ]);
        var catalog = new CatalogModule(
        [
            new GameSnapshot(
                availableGameId,
                "Cloud Quest",
                10m,
                true)
        ]);
        var service = new GetLibraryService(
            queries,
            catalog,
            NullLogger<GetLibraryService>.Instance);

        var result = await service.ExecuteAsync(
            userId,
            CancellationToken.None);

        Assert.Equal(1, catalog.BatchCallCount);
        Assert.Equal(0, catalog.SingleCallCount);
        Assert.Equal(
            [availableGameId, unavailableGameId],
            catalog.RequestedIds);
        var resultGames = result.Games.ToArray();
        Assert.Equal("Cloud Quest", resultGames[0].GameTitle);
        Assert.Equal("Jogo indisponível", resultGames[1].GameTitle);
    }

    private sealed class LibraryQueries(
        IReadOnlyList<LibraryGameReadModel> games) : ILibraryQueries
    {
        public Task<IReadOnlyList<LibraryGameReadModel>> ListGamesAsync(
            Guid userId,
            CancellationToken cancellationToken) =>
            Task.FromResult(games);
    }

    private sealed class CatalogModule(
        IReadOnlyList<GameSnapshot> games) : ICatalogModule
    {
        public int SingleCallCount { get; private set; }
        public int BatchCallCount { get; private set; }
        public IReadOnlyCollection<Guid> RequestedIds { get; private set; } = [];

        public Task<GameSnapshot?> GetGameAsync(
            GetGameQuery query,
            CancellationToken cancellationToken)
        {
            SingleCallCount++;
            throw new InvalidOperationException(
                "A consulta individual não deveria ser executada.");
        }

        public Task<IReadOnlyList<GameSnapshot>> GetGamesAsync(
            GetGamesQuery query,
            CancellationToken cancellationToken)
        {
            BatchCallCount++;
            RequestedIds = query.GameIds;
            return Task.FromResult(games);
        }
    }
}
