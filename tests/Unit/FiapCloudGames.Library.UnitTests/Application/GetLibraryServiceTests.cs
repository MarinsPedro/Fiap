using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Library.Application.Abstractions.Queries;
using FiapCloudGames.Library.Application.Features.UserLibrary.GetCurrentLibrary;
using FiapCloudGames.Library.Application.Features.UserLibrary.GetLibrary;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Library.UnitTests.Application;

public sealed class GetLibraryServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithEmptyLibrary_ShouldReturnEmptyResultWithoutCatalogCall()
    {
        var userId = Guid.NewGuid();
        var queries = new StubLibraryQueries();
        var catalog = new StubCatalogModule();
        var service = CreateService(queries, catalog);

        var result = await service.ExecuteAsync(
            userId,
            CancellationToken.None);

        Assert.Equal(userId, result.UserId);
        Assert.Empty(result.Games);
        Assert.Equal(userId, queries.RequestedUserId);
        Assert.Equal(0, catalog.BatchCalls);
    }

    [Fact]
    public async Task ExecuteAsync_WithGames_ShouldLoadCatalogInSingleBatch()
    {
        var userId = Guid.NewGuid();
        var availableGameId = Guid.NewGuid();
        var unavailableGameId = Guid.NewGuid();
        var queries = new StubLibraryQueries
        {
            Result =
            [
                new LibraryGameReadModel(
                    Guid.NewGuid(),
                    availableGameId,
                    10m,
                    null,
                    LibraryTestData.NowUtc),
                new LibraryGameReadModel(
                    Guid.NewGuid(),
                    unavailableGameId,
                    20m,
                    null,
                    LibraryTestData.NowUtc.AddHours(-1))
            ]
        };
        var catalog = new StubCatalogModule
        {
            GamesResult =
            [
                new GameSnapshot(
                    availableGameId,
                    "Cloud Quest",
                    10m,
                    true)
            ]
        };
        var service = CreateService(queries, catalog);

        var result = await service.ExecuteAsync(
            userId,
            CancellationToken.None);

        Assert.Equal(1, catalog.BatchCalls);
        Assert.Equal(0, catalog.SingleCalls);
        Assert.Equal(
            [availableGameId, unavailableGameId],
            catalog.RequestedGameIds);
        Assert.Collection(
            result.Games,
            item => Assert.Equal("Cloud Quest", item.GameTitle),
            item => Assert.Equal("Jogo indisponível", item.GameTitle));
    }

    [Fact]
    public async Task GetCurrentLibrary_WithAuthenticatedUser_ShouldReturnThatLibrary()
    {
        var userId = Guid.NewGuid();
        var queries = new StubLibraryQueries();
        var service = new GetCurrentLibraryService(
            new StubCurrentUserContext(userId),
            CreateService(queries, new StubCatalogModule()));

        var result = await service.ExecuteAsync(CancellationToken.None);

        Assert.Equal(userId, result.UserId);
        Assert.Equal(userId, queries.RequestedUserId);
    }

    [Fact]
    public async Task GetCurrentLibrary_WithoutAuthenticatedUser_ShouldThrowAuthentication()
    {
        var service = new GetCurrentLibraryService(
            new StubCurrentUserContext(null),
            CreateService(
                new StubLibraryQueries(),
                new StubCatalogModule()));

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(CancellationToken.None));

        Assert.Equal(AppErrorCategory.Authentication, exception.Category);
    }

    private static GetLibraryService CreateService(
        StubLibraryQueries queries,
        StubCatalogModule catalog) =>
        new(
            queries,
            catalog,
            NullLogger<GetLibraryService>.Instance);
}
