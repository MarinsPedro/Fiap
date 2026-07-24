using FiapCloudGames.Catalog.Application.Abstractions;
using FiapCloudGames.Catalog.Contracts;
using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Catalog.Domain.Repositories;

namespace FiapCloudGames.Catalog.Application.Games;

public sealed record CreateGameInput(string Title, string Description, string Category, decimal BasePrice);
public sealed record UpdateGameInput(string Title, string Description, string Category, decimal BasePrice, bool IsActive);

public sealed class CreateGameService(IGameRepository games, ICatalogUnitOfWork unitOfWork)
{
    public async Task<GameSummary> ExecuteAsync(CreateGameInput input, CancellationToken cancellationToken)
    {
        var game = Game.Create(input.Title, input.Description, input.Category, input.BasePrice);
        await games.AddAsync(game, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return CatalogMappings.ToSummary(game);
    }
}

public sealed class UpdateGameService(IGameRepository games, ICatalogUnitOfWork unitOfWork)
{
    public async Task<GameSummary> ExecuteAsync(
        Guid id,
        UpdateGameInput input,
        CancellationToken cancellationToken)
    {
        var game = await games.GetAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Jogo não encontrado.");
        game.Update(input.Title, input.Description, input.Category, input.BasePrice);
        game.SetActive(input.IsActive);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return CatalogMappings.ToSummary(game);
    }
}

public sealed class GetGameService(IGameRepository games)
{
    public async Task<GameSummary?> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var game = await games.GetAsync(id, cancellationToken);
        return game is null ? null : CatalogMappings.ToSummary(game);
    }
}

public sealed class ListGamesService(IGameRepository games)
{
    public async Task<IReadOnlyList<GameSummary>> ExecuteAsync(
        bool onlyActive,
        CancellationToken cancellationToken) =>
        (await games.ListAsync(onlyActive, cancellationToken)).Select(CatalogMappings.ToSummary).ToArray();
}

internal sealed class CatalogModule(GetGameService getGameService) : ICatalogModule
{
    public Task<GameSummary?> GetGameAsync(Guid gameId, CancellationToken cancellationToken) =>
        getGameService.ExecuteAsync(gameId, cancellationToken);
}

internal static class CatalogMappings
{
    public static GameSummary ToSummary(Game game) =>
        new(game.Id, game.Title, game.Description, game.Category, game.BasePrice, game.IsActive);
}
