namespace FiapCloudGames.Library.Application.Features.UserLibrary;

public sealed record UserLibraryResult(
    Guid UserId,
    IReadOnlyCollection<LibraryItemResult> Games);
