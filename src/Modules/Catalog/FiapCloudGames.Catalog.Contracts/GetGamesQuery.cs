namespace FiapCloudGames.Catalog.Contracts;

public sealed record GetGamesQuery(
    IReadOnlyCollection<Guid> GameIds);
