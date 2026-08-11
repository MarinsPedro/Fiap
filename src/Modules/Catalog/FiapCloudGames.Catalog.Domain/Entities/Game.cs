using FiapCloudGames.Catalog.Domain.ValueObjects;
using FiapCloudGames.Domain.Common;

namespace FiapCloudGames.Catalog.Domain.Entities;

public sealed class Game
{
    private Game()
    {
    }

    private Game(
        Guid id,
        string title,
        string description,
        string category,
        decimal basePrice,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        ChangeDetails(
            title,
            description,
            category,
            basePrice);
        IsActive = true;
        CreatedAtUtc = ValidateCreatedAt(createdAtUtc);
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public GamePrice BasePrice { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static Game Create(
        string title,
        string description,
        string category,
        decimal basePrice,
        DateTimeOffset createdAtUtc) =>
        new(
            Guid.NewGuid(),
            title,
            description,
            category,
            basePrice,
            createdAtUtc);

    public void ChangeDetails(
        string title,
        string description,
        string category,
        decimal basePrice)
    {
        Title = NormalizeTitle(title);
        Description = NormalizeDescription(description);
        Category = NormalizeCategory(category);
        BasePrice = GamePrice.Create(basePrice);
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string NormalizeTitle(string? title) =>
        title?.Trim() is { Length: >= 2 and <= 160 } normalized
            ? normalized
            : throw new DomainRuleViolationException(
                "O título deve ter entre 2 e 160 caracteres.");

    private static string NormalizeDescription(string? description) =>
        (description?.Trim() ?? string.Empty) is
            { Length: <= 4000 } normalized
            ? normalized
            : throw new DomainRuleViolationException(
                "A descrição deve ter no máximo 4000 caracteres.");

    private static string NormalizeCategory(string? category) =>
        category?.Trim() is { Length: >= 1 and <= 80 } normalized
            ? normalized
            : throw new DomainRuleViolationException(
                "A categoria é obrigatória e deve ter no máximo 80 caracteres.");

    private static DateTimeOffset ValidateCreatedAt(
        DateTimeOffset createdAtUtc) =>
        createdAtUtc != default && createdAtUtc.Offset == TimeSpan.Zero
            ? createdAtUtc
            : throw new DomainRuleViolationException(
                "A data de criação do jogo deve estar em UTC.");
}
