namespace FiapCloudGames.Library.Presentation.Features.UserLibrary;

public sealed record UserLibraryResponse(
    Guid UserId,
    IReadOnlyCollection<LibraryItemResponse> Games);
