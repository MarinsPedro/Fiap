namespace FiapCloudGames.Catalog.Contracts;

public sealed record GameSnapshot(
    Guid Id,
    string Title,
    decimal BasePrice,
    bool IsActive);
