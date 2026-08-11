namespace FiapCloudGames.Catalog.Presentation.Features.Games;

public sealed record GameResponse(
    Guid Id,
    string Title,
    string Description,
    string Category,
    decimal BasePrice,
    bool IsActive);
