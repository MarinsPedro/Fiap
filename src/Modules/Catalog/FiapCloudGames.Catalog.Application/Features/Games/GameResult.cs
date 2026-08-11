namespace FiapCloudGames.Catalog.Application.Features.Games;

public sealed record GameResult(
    Guid Id,
    string Title,
    string Description,
    string Category,
    decimal BasePrice,
    bool IsActive);
