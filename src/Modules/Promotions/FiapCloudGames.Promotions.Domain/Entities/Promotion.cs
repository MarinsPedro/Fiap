using FiapCloudGames.Domain.Common;
using FiapCloudGames.Promotions.Domain.ValueObjects;

namespace FiapCloudGames.Promotions.Domain.Entities;

public sealed class Promotion
{
    private readonly List<PromotionGame> _games = [];

    private Promotion()
    {
    }

    private Promotion(
        Guid id,
        string name,
        decimal discountPercent,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        IEnumerable<Guid> gameIds,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        Name = ValidateName(name);
        DiscountPercent = DiscountPercentage.Create(
            discountPercent);

        StartsAtUtc = ValidateStartsAt(startsAtUtc);
        EndsAtUtc = ValidateEndsAt(endsAtUtc);
        CreatedAtUtc = ValidateCreatedAt(createdAtUtc);

        EnsureValidPeriod();
        AddGames(id, gameIds);
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DiscountPercentage DiscountPercent { get; private set; } =
        null!;
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
        IEnumerable<Guid> gameIds,
        DateTimeOffset createdAtUtc) =>
        new(
            Guid.NewGuid(),
            name,
            discountPercent,
            startsAtUtc,
            endsAtUtc,
            gameIds,
            createdAtUtc);

    public bool IsActiveAt(DateTimeOffset instant) =>
        EndedAtUtc is null &&
        StartsAtUtc <= instant &&
        instant < EndsAtUtc;

    public bool Includes(Guid gameId) =>
        _games.Any(item => item.GameId == gameId);

    public decimal ApplyTo(decimal basePrice) =>
        DiscountPercent.ApplyTo(basePrice);

    public void End(DateTimeOffset instant)
    {
        if (EndedAtUtc is not null)
        {
            return;
        }

        var endedAtUtc = ValidateEndedAt(instant);
        if (endedAtUtc < CreatedAtUtc)
        {
            throw new DomainRuleViolationException(
                "A promoção não pode terminar antes de ser criada.");
        }

        EndedAtUtc = endedAtUtc;
    }

    private static string ValidateName(string? name) =>
        name?.Trim() is { Length: >= 2 and <= 120 } normalized
            ? normalized
            : throw new DomainRuleViolationException(
                "O nome da promoção deve ter entre 2 e 120 caracteres.");

    private static DateTimeOffset ValidateStartsAt(
        DateTimeOffset startsAtUtc)
    {
        if (startsAtUtc == default ||
            startsAtUtc.Offset != TimeSpan.Zero)
        {
            throw new DomainRuleViolationException(
                "O início da promoção deve estar em UTC.");
        }

        return startsAtUtc;
    }

    private static DateTimeOffset ValidateEndsAt(
        DateTimeOffset endsAtUtc)
    {
        if (endsAtUtc == default ||
            endsAtUtc.Offset != TimeSpan.Zero)
        {
            throw new DomainRuleViolationException(
                "O fim da promoção deve estar em UTC.");
        }

        return endsAtUtc;
    }

    private static DateTimeOffset ValidateCreatedAt(
        DateTimeOffset createdAtUtc)
    {
        if (createdAtUtc == default ||
            createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new DomainRuleViolationException(
                "A data de criação da promoção deve estar em UTC.");
        }

        return createdAtUtc;
    }

    private static DateTimeOffset ValidateEndedAt(
        DateTimeOffset endedAtUtc)
    {
        if (endedAtUtc == default ||
            endedAtUtc.Offset != TimeSpan.Zero)
        {
            throw new DomainRuleViolationException(
                "A data de encerramento deve estar em UTC.");
        }

        return endedAtUtc;
    }

    private void EnsureValidPeriod()
    {
        if (EndsAtUtc <= StartsAtUtc)
        {
            throw new DomainRuleViolationException(
                "O fim da promoção deve ser posterior ao início.");
        }
    }

    private void AddGames(
        Guid promotionId,
        IEnumerable<Guid>? gameIds)
    {
        if (gameIds is null)
        {
            throw GamesRequired();
        }

        foreach (var gameId in gameIds.Distinct())
        {
            if (gameId == Guid.Empty)
            {
                throw new DomainRuleViolationException(
                    "Todos os identificadores de jogo devem ser válidos.");
            }

            _games.Add(new PromotionGame(promotionId, gameId));
        }

        if (_games.Count == 0)
        {
            throw GamesRequired();
        }
    }

    private static DomainRuleViolationException GamesRequired() =>
        new(
            "A promoção deve possuir pelo menos um jogo.");
}
