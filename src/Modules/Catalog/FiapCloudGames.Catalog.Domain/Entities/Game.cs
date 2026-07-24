namespace FiapCloudGames.Catalog.Domain.Entities;

public sealed class Game
{
    private Game() { }

    private Game(Guid id, string title, string description, string category, decimal basePrice)
    {
        Id = id;
        Update(title, description, category, basePrice);
        IsActive = true;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static Game Create(string title, string description, string category, decimal basePrice) =>
        new(Guid.NewGuid(), title, description, category, basePrice);

    public void Update(string title, string description, string category, decimal basePrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        var normalizedTitle = title.Trim();
        if (normalizedTitle.Length is < 2 or > 160)
        {
            throw new InvalidOperationException("O título deve ter entre 2 e 160 caracteres.");
        }

        if (basePrice < 0)
        {
            throw new InvalidOperationException("O preço base não pode ser negativo.");
        }

        Title = normalizedTitle;
        Description = description?.Trim() ?? string.Empty;
        Category = category.Trim();
        BasePrice = decimal.Round(basePrice, 2, MidpointRounding.ToEven);
    }

    public void SetActive(bool isActive) => IsActive = isActive;
}
