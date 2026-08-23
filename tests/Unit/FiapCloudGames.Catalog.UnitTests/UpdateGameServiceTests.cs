using FiapCloudGames.Catalog.Application.Abstractions.Persistence;
using FiapCloudGames.Catalog.Application.Features.Games.UpdateGame;
using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Catalog.UnitTests;

public sealed class UpdateGameServiceTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateShouldPreserveLifecycleState(bool isActive)
    {
        var game = Game.Create(
            "Cloud Quest",
            "Aventura",
            "RPG",
            10m,
            CreatedAtUtc);
        if (!isActive)
        {
            game.Deactivate();
        }

        var unitOfWork = new CatalogUnitOfWork();
        var service = new UpdateGameService(
            new GameRepository(game),
            unitOfWork,
            NullLogger<UpdateGameService>.Instance);

        await service.ExecuteAsync(
            game.Id,
            new UpdateGameInput(
                "Cloud Quest Deluxe",
                "Edição atualizada",
                "RPG",
                20m),
            CancellationToken.None);

        Assert.Equal(isActive, game.IsActive);
        Assert.Equal("Cloud Quest Deluxe", game.Title);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    private sealed class CatalogUnitOfWork : ICatalogUnitOfWork
    {
        public int SaveChangesCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            SaveChangesCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class GameRepository(Game game) : IGameRepository
    {
        public Task AddAsync(
            Game gameToAdd,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Game?> GetAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            Task.FromResult<Game?>(id == game.Id ? game : null);

        public Task<IReadOnlyList<Game>> ListByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<Game>> ListAsync(
            bool onlyActive,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
