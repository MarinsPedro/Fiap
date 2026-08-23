using FiapCloudGames.Application.Common.Authentication;
using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Identity.Contracts;
using FiapCloudGames.Library.Application.Abstractions.Persistence;
using FiapCloudGames.Library.Application.Abstractions.Queries;
using FiapCloudGames.Library.Domain.Entities;
using FiapCloudGames.Library.Domain.Repositories;
using FiapCloudGames.Promotions.Contracts;

namespace FiapCloudGames.Library.UnitTests.Application;

internal static class LibraryTestData
{
    internal static readonly DateTimeOffset NowUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);
}

internal sealed class FakeGameLibraryRepository : IGameLibraryRepository
{
    internal GameLibrary? Library { get; set; }
    internal GameLibrary? AddedLibrary { get; private set; }
    internal Guid? RequestedUserId { get; private set; }
    internal int AddCalls { get; private set; }

    public Task AddAsync(
        GameLibrary library,
        CancellationToken cancellationToken)
    {
        AddCalls++;
        AddedLibrary = library;
        return Task.CompletedTask;
    }

    public Task<GameLibrary?> GetByUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        RequestedUserId = userId;
        return Task.FromResult(
            Library?.UserId == userId
                ? Library
                : null);
    }
}

internal sealed class SpyLibraryUnitOfWork : ILibraryUnitOfWork
{
    internal int SaveChangesCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}

internal sealed class StubIdentityModule : IIdentityModule
{
    internal UserSnapshot? Result { get; set; }
    internal Guid? RequestedUserId { get; private set; }
    internal int Calls { get; private set; }

    public Task<UserSnapshot?> GetUserAsync(
        GetUserQuery query,
        CancellationToken cancellationToken)
    {
        Calls++;
        RequestedUserId = query.UserId;
        return Task.FromResult(Result);
    }
}

internal sealed class StubCatalogModule : ICatalogModule
{
    internal GameSnapshot? GameResult { get; set; }
    internal IReadOnlyList<GameSnapshot> GamesResult { get; set; } = [];
    internal Guid? RequestedGameId { get; private set; }
    internal IReadOnlyCollection<Guid> RequestedGameIds { get; private set; } = [];
    internal int SingleCalls { get; private set; }
    internal int BatchCalls { get; private set; }

    public Task<GameSnapshot?> GetGameAsync(
        GetGameQuery query,
        CancellationToken cancellationToken)
    {
        SingleCalls++;
        RequestedGameId = query.GameId;
        return Task.FromResult(GameResult);
    }

    public Task<IReadOnlyList<GameSnapshot>> GetGamesAsync(
        GetGamesQuery query,
        CancellationToken cancellationToken)
    {
        BatchCalls++;
        RequestedGameIds = query.GameIds;
        return Task.FromResult(GamesResult);
    }
}

internal sealed class StubPromotionsModule : IPromotionsModule
{
    internal PriceQuoteSnapshot Result { get; set; } =
        new(100m, 100m, 0m, null);
    internal GetPriceQuoteQuery? RequestedQuery { get; private set; }
    internal int Calls { get; private set; }

    public Task<PriceQuoteSnapshot> GetPriceAsync(
        GetPriceQuoteQuery query,
        CancellationToken cancellationToken)
    {
        Calls++;
        RequestedQuery = query;
        return Task.FromResult(Result);
    }
}

internal sealed class StubLibraryQueries : ILibraryQueries
{
    internal IReadOnlyList<LibraryGameReadModel> Result { get; set; } = [];
    internal Guid? RequestedUserId { get; private set; }

    public Task<IReadOnlyList<LibraryGameReadModel>> ListGamesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        RequestedUserId = userId;
        return Task.FromResult(Result);
    }
}

internal sealed record StubCurrentUserContext(Guid? UserId) :
    ICurrentUserContext;

internal sealed class FixedTimeProvider(DateTimeOffset nowUtc) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => nowUtc;
}
