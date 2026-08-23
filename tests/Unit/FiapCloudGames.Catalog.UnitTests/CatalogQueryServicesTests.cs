using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Application.Features.Games.FindGame;
using FiapCloudGames.Catalog.Application.Features.Games.FindGames;
using FiapCloudGames.Catalog.Application.Features.Games.GetGame;
using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Catalog.UnitTests;

public sealed class CatalogQueryServicesTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task FindGameShouldReturnNullWhenGameDoesNotExist()
    {
        var service = new FindGameService(
            new GameRepository([]),
            NullLogger<FindGameService>.Instance);

        var result = await service.ExecuteAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetGameShouldKeepNotFoundSemanticsForHttpUseCase()
    {
        var findGame = new FindGameService(
            new GameRepository([]),
            NullLogger<FindGameService>.Instance);
        var service = new GetGameService(findGame);

        var exception = await Assert.ThrowsAsync<AppException>(
            () => service.ExecuteAsync(
                Guid.NewGuid(),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
    }

    [Fact]
    public async Task FindGamesShouldUseSingleBatchRepositoryCall()
    {
        var games = new[]
        {
            CreateGame("Cloud Quest"),
            CreateGame("FIAP Arena")
        };
        var repository = new GameRepository(games);
        var service = new FindGamesService(
            repository,
            NullLogger<FindGamesService>.Instance);

        var results = await service.ExecuteAsync(
            games.Select(game => game.Id).ToArray(),
            CancellationToken.None);

        Assert.Equal(2, results.Count);
        Assert.Equal(1, repository.ListByIdsCallCount);
        Assert.Equal(
            games.Select(game => game.Id).Order(),
            repository.RequestedIds.Order());
    }

    private static Game CreateGame(string title) =>
        Game.Create(
            title,
            "Descrição",
            "Testes",
            10m,
            CreatedAtUtc);

    private sealed class GameRepository(
        IReadOnlyCollection<Game> games) : IGameRepository
    {
        public int ListByIdsCallCount { get; private set; }
        public IReadOnlyCollection<Guid> RequestedIds { get; private set; } = [];

        public Task AddAsync(
            Game game,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Game?> GetAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            Task.FromResult(games.SingleOrDefault(game => game.Id == id));

        public Task<IReadOnlyList<Game>> ListByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken)
        {
            ListByIdsCallCount++;
            RequestedIds = ids;
            return Task.FromResult<IReadOnlyList<Game>>(
                games.Where(game => ids.Contains(game.Id)).ToArray());
        }

        public Task<IReadOnlyList<Game>> ListAsync(
            bool onlyActive,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
