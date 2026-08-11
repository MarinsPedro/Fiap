namespace FiapCloudGames.Library.Contracts;

public interface ILibraryModule
{
    Task<UserLibrarySnapshot> GetLibraryAsync(
        GetUserLibraryQuery query,
        CancellationToken cancellationToken);
}
