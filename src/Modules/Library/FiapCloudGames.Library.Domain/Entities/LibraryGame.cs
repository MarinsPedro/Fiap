using FiapCloudGames.Library.Domain.ValueObjects;

namespace FiapCloudGames.Library.Domain.Entities;

public sealed class LibraryGame
{
    private LibraryGame()
    {
    }

    internal LibraryGame(
        Guid id,
        Guid libraryId,
        Guid gameId,
        AcquisitionPrice pricePaid,
        Guid? promotionId,
        DateTimeOffset acquiredAtUtc)
    {
        Id = id;
        LibraryId = libraryId;
        GameId = gameId;
        PricePaid = pricePaid;
        PromotionId = promotionId;
        AcquiredAtUtc = acquiredAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid LibraryId { get; private set; }
    public Guid GameId { get; private set; }
    public AcquisitionPrice PricePaid { get; private set; }
    public Guid? PromotionId { get; private set; }
    public DateTimeOffset AcquiredAtUtc { get; private set; }
}
