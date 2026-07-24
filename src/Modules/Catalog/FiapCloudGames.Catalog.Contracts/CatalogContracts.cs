namespace FiapCloudGames.Catalog.Contracts;

public sealed record GameSummary(
    Guid Id,
    string Title,
    string Description,
    string Category,
    decimal BasePrice,
    bool IsActive);

public interface ICatalogModule
{
    Task<GameSummary?> GetGameAsync(Guid gameId, CancellationToken cancellationToken);
}

public sealed record GameDeactivatedIntegrationEvent(Guid GameId, DateTimeOffset OccurredAtUtc);
