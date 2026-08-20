namespace FiapCloudGames.Library.Presentation.Features.UserLibrary;

/// <summary>
/// Representa a resposta contendo uma coleção (biblioteca) de jogos do usuário.
/// </summary>
/// <param name="UserId">O ID do usuário.</param>
/// <param name="Games">A coleção (biblioteca) de jogos do usuário.</param>
public sealed record UserLibraryResponse(
    Guid UserId,
    IReadOnlyCollection<LibraryItemResponse> Games);
