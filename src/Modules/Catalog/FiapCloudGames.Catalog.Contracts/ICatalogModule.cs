namespace FiapCloudGames.Catalog.Contracts;

public interface ICatalogModule
{
    Task<GameSnapshot?> GetGameAsync(
        GetGameQuery query,
        CancellationToken cancellationToken);
}
