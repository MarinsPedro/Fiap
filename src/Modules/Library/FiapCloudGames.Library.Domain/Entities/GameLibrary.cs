namespace FiapCloudGames.Library.Domain.Entities;

public sealed class GameLibrary
{
    private readonly List<LibraryGame> _games = [];

    private GameLibrary() { }

    private GameLibrary(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<LibraryGame> Games => _games;

    public static GameLibrary Create(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("O usuário é obrigatório.", nameof(userId));
        }

        return new GameLibrary(Guid.NewGuid(), userId);
    }

    public LibraryGame AddGame(Guid gameId, decimal pricePaid, Guid? promotionId, DateTimeOffset acquiredAtUtc)
    {
        if (_games.Any(item => item.GameId == gameId))
        {
            throw new InvalidOperationException("O jogo já pertence à biblioteca do usuário.");
        }

        if (pricePaid < 0)
        {
            throw new InvalidOperationException("O preço pago não pode ser negativo.");
        }

        var item = new LibraryGame(
            Guid.NewGuid(),
            Id,
            gameId,
            decimal.Round(pricePaid, 2, MidpointRounding.ToEven),
            promotionId,
            acquiredAtUtc);
        _games.Add(item);
        return item;
    }
}

public sealed class LibraryGame
{
    private LibraryGame() { }

    internal LibraryGame(
        Guid id,
        Guid libraryId,
        Guid gameId,
        decimal pricePaid,
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
    public decimal PricePaid { get; private set; }
    public Guid? PromotionId { get; private set; }
    public DateTimeOffset AcquiredAtUtc { get; private set; }
}
