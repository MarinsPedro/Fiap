using FiapCloudGames.Identity.Domain.Enums;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.Domain.Entities;

public sealed class User
{
    private User() { }

    private User(Guid id, string name, Email email, string passwordHash, UserRole role)
    {
        Id = id;
        ChangeName(name);
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static User Create(string name, Email email, string passwordHash, UserRole role = UserRole.User)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        return new User(Guid.NewGuid(), name, email, passwordHash, role);
    }

    public void ChangeName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var trimmed = name.Trim();
        if (trimmed.Length is < 2 or > 120)
        {
            throw new InvalidOperationException("O nome deve ter entre 2 e 120 caracteres.");
        }

        Name = trimmed;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
    }
}
