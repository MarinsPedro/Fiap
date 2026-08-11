namespace FiapCloudGames.Library.Application.Abstractions.Queries;

public interface ILibraryQueries
{
    Task<IReadOnlyList<LibraryGameReadModel>> ListGamesAsync(
        Guid userId,
        CancellationToken cancellationToken);
}
