using FiapCloudGames.Catalog.Application.Abstractions.Persistence;
using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Catalog.Domain.Repositories;

namespace FiapCloudGames.Catalog.UnitTests.Application;

internal static class CatalogTestData
{
    internal static readonly DateTimeOffset NowUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    internal static Game CreateGame(string title = "Cloud Quest") =>
        Game.Create(
            title,
            "Descrição",
            "Testes",
            10m,
            NowUtc);
}

internal sealed class FakeGameRepository : IGameRepository
{
    internal Game? Game { get; set; }
    internal IReadOnlyList<Game> Games { get; set; } = [];
    internal Game? AddedGame { get; private set; }
    internal IReadOnlyCollection<Guid> RequestedIds { get; private set; } = [];
    internal bool? RequestedOnlyActive { get; private set; }
    internal int AddCalls { get; private set; }
    internal int ListByIdsCalls { get; private set; }

    public Task AddAsync(Game game, CancellationToken cancellationToken)
    {
        AddCalls++;
        AddedGame = game;
        return Task.CompletedTask;
    }

    public Task<Game?> GetAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        Task.FromResult(
            Game?.Id == id
                ? Game
                : null);

    public Task<IReadOnlyList<Game>> ListByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        ListByIdsCalls++;
        RequestedIds = ids;
        return Task.FromResult<IReadOnlyList<Game>>(
            Games.Where(game => ids.Contains(game.Id)).ToArray());
    }

    public Task<IReadOnlyList<Game>> ListAsync(
        bool onlyActive,
        CancellationToken cancellationToken)
    {
        RequestedOnlyActive = onlyActive;
        return Task.FromResult(Games);
    }
}

internal sealed class SpyCatalogUnitOfWork : ICatalogUnitOfWork
{
    internal int SaveChangesCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset nowUtc) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => nowUtc;
}
