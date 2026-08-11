using FiapCloudGames.Domain.Common;
using FiapCloudGames.Library.Domain.ValueObjects;

namespace FiapCloudGames.Library.Domain.Entities;

public sealed class GameLibrary
{
    private readonly List<LibraryGame> _games = [];

    private GameLibrary()
    {
    }

    private GameLibrary(
        Guid id,
        Guid userId,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        UserId = ValidateUserId(userId);
        CreatedAtUtc = ValidateCreatedAt(createdAtUtc);
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<LibraryGame> Games => _games;

    public static GameLibrary Create(
        Guid userId,
        DateTimeOffset createdAtUtc) =>
        new(
            Guid.NewGuid(),
            userId,
            createdAtUtc);

    public bool ContainsGame(Guid gameId) =>
        _games.Any(item => item.GameId == gameId);

    public LibraryGame AcquireGame(
        Guid gameId,
        decimal pricePaid,
        Guid? promotionId,
        DateTimeOffset acquiredAtUtc)
    {
        ValidateGameId(gameId);
        EnsureGameIsNotOwned(gameId);
        ValidatePromotionId(promotionId);
        var acquisitionTime = ValidateAcquiredAt(acquiredAtUtc);

        var item = new LibraryGame(
            Guid.NewGuid(),
            Id,
            gameId,
            AcquisitionPrice.Create(pricePaid),
            promotionId,
            acquisitionTime);

        _games.Add(item);
        return item;
    }

    private static Guid ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainRuleViolationException(
                "O usuário é obrigatório.");
        }

        return userId;
    }

    private static DateTimeOffset ValidateCreatedAt(
        DateTimeOffset createdAtUtc)
    {
        if (createdAtUtc == default ||
            createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new DomainRuleViolationException(
                "A data de criação da biblioteca deve estar em UTC.");
        }

        return createdAtUtc;
    }

    private static void ValidateGameId(Guid gameId)
    {
        if (gameId == Guid.Empty)
        {
            throw new DomainRuleViolationException(
                "O jogo é obrigatório.");
        }
    }

    private void EnsureGameIsNotOwned(Guid gameId)
    {
        if (ContainsGame(gameId))
        {
            throw new DomainRuleViolationException(
                "O jogo já pertence à biblioteca do usuário.");
        }
    }

    private static void ValidatePromotionId(Guid? promotionId)
    {
        if (promotionId == Guid.Empty)
        {
            throw new DomainRuleViolationException(
                "A promoção informada é inválida.");
        }
    }

    private DateTimeOffset ValidateAcquiredAt(
        DateTimeOffset acquiredAtUtc)
    {
        if (acquiredAtUtc == default ||
            acquiredAtUtc.Offset != TimeSpan.Zero)
        {
            throw new DomainRuleViolationException(
                "A data de aquisição deve estar em UTC.");
        }

        if (acquiredAtUtc < CreatedAtUtc)
        {
            throw new DomainRuleViolationException(
                "A aquisição não pode ocorrer antes da criação da biblioteca.");
        }

        return acquiredAtUtc;
    }
}
