namespace FiapCloudGames.Promotions.Domain.Entities;

public sealed class Promotion
{
    private readonly List<PromotionGame> _games = [];

    private Promotion() { }

    private Promotion(
        Guid id,
        string name,
        decimal discountPercent,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        IEnumerable<Guid> gameIds)
    {
        Id = id;
        Name = ValidateName(name);
        DiscountPercent = ValidateDiscount(discountPercent);
        ValidatePeriod(startsAtUtc, endsAtUtc);
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        CreatedAtUtc = DateTimeOffset.UtcNow;

        foreach (var gameId in gameIds.Distinct())
        {
            _games.Add(new PromotionGame(id, gameId));
        }

        if (_games.Count == 0)
        {
            throw new InvalidOperationException("A promoção deve possuir pelo menos um jogo.");
        }
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal DiscountPercent { get; private set; }
    public DateTimeOffset StartsAtUtc { get; private set; }
    public DateTimeOffset EndsAtUtc { get; private set; }
    public DateTimeOffset? EndedAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<PromotionGame> Games => _games;

    public static Promotion Create(
        string name,
        decimal discountPercent,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        IEnumerable<Guid> gameIds) =>
        new(Guid.NewGuid(), name, discountPercent, startsAtUtc, endsAtUtc, gameIds);

    public bool IsActiveAt(DateTimeOffset instant) =>
        EndedAtUtc is null && StartsAtUtc <= instant && instant < EndsAtUtc;

    public bool Includes(Guid gameId) => _games.Any(item => item.GameId == gameId);

    public decimal ApplyTo(decimal basePrice) =>
        decimal.Round(basePrice * (1 - (DiscountPercent / 100m)), 2, MidpointRounding.ToEven);

    public void End(DateTimeOffset instant)
    {
        if (EndedAtUtc is not null)
        {
            return;
        }

        EndedAtUtc = instant;
    }

    private static string ValidateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var trimmed = name.Trim();
        if (trimmed.Length is < 2 or > 120)
        {
            throw new InvalidOperationException("O nome da promoção deve ter entre 2 e 120 caracteres.");
        }

        return trimmed;
    }

    private static decimal ValidateDiscount(decimal discount)
    {
        if (discount is <= 0 or > 100)
        {
            throw new InvalidOperationException("O desconto deve ser maior que zero e menor ou igual a 100%.");
        }

        return decimal.Round(discount, 2, MidpointRounding.ToEven);
    }

    private static void ValidatePeriod(DateTimeOffset startsAtUtc, DateTimeOffset endsAtUtc)
    {
        if (endsAtUtc <= startsAtUtc)
        {
            throw new InvalidOperationException("O fim da promoção deve ser posterior ao início.");
        }
    }
}

public sealed class PromotionGame
{
    private PromotionGame() { }

    internal PromotionGame(Guid promotionId, Guid gameId)
    {
        PromotionId = promotionId;
        GameId = gameId;
    }

    public Guid PromotionId { get; private set; }
    public Guid GameId { get; private set; }
}
