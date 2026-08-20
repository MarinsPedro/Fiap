using FiapCloudGames.Library.Application.Features.UserLibrary;

namespace FiapCloudGames.Library.Presentation.Features.UserLibrary;

/// <summary>
/// Classe de mapeamento de request para resposta de item de biblioteca e da biblioteca inteira de jogos do usuário.
/// </summary>
internal static class LibraryHttpMappings
{
    public static LibraryItemResponse ToResponse(
        this LibraryItemResult result) =>
        new(
            result.Id,
            result.GameId,
            result.GameTitle,
            result.PricePaid,
            result.PromotionId,
            result.AcquiredAtUtc);

    public static UserLibraryResponse ToResponse(
        this UserLibraryResult result) =>
        new(
            result.UserId,
            result.Games
                .Select(ToResponse)
                .ToArray());
}
