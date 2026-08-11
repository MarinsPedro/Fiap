namespace FiapCloudGames.Library.Contracts;

public sealed record UserLibrarySnapshot(
    Guid UserId,
    IReadOnlyCollection<LibraryItemSnapshot> Games);
