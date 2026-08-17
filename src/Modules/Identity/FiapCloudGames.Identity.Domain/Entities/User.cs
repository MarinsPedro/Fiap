using System.Data;
using FiapCloudGames.Domain.Common;
using FiapCloudGames.Identity.Domain.Enums;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.Domain.Entities;

public sealed class User
{
    private User() { }

    private User(
        Guid id,
        string name,
        Email email,
        string passwordHash,
        UserRole role,
        DateTimeOffset createdAtUtc)
    {
        Id = ValidateId(id);
        ChangeName(name);
        Email = email ?? throw new DomainRuleViolationException("O e-mail do usuário é obrigatório.");
        PasswordHash = ValidatePasswordHash(passwordHash);

        if (role is UserRole.Undefined || !Enum.IsDefined(role))
        {
            throw new DomainRuleViolationException("O perfil do usuário é inválido.");
        }

        Role = role;
        IsActive = true;
        CreatedAtUtc = ValidateCreatedAt(createdAtUtc);
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static User Create(
        string name,
        Email email,
        string passwordHash,
        DateTimeOffset createdAtUtc,
        UserRole role = UserRole.User) =>
        new(
            Guid.NewGuid(),
            name,
            email,
            passwordHash,
            role,
            createdAtUtc);

    public void ChangeDetails(
        string name,
        Email email)
    {
        ChangeName(name);
        Email = email ?? throw new DomainRuleViolationException("O e-mail do usuário é obrigatório.");
    }

    public void ChangeName(string name)
        => Name = NormalizeName(name);

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
    }

    private static Guid ValidateId(Guid id) =>
        id != Guid.Empty
            ? id
            : throw new DomainRuleViolationException(
                "O identificador do usuário é obrigatório.");

    private static string NormalizeName(string? name) =>
        name?.Trim() is { Length: >= 2 and <= 120 } normalized
            ? normalized
            : throw new DomainRuleViolationException(
                "O nome deve ter entre 2 e 120 caracteres.");

    private static string ValidatePasswordHash(string? passwordHash) =>
        passwordHash?.Trim() is { Length: > 0 } normalized
            ? normalized
            : throw new DomainRuleViolationException(
                "O hash da senha é obrigatório.");

    private static DateTimeOffset ValidateCreatedAt(
        DateTimeOffset createdAtUtc) =>
        createdAtUtc != default && createdAtUtc.Offset == TimeSpan.Zero
            ? createdAtUtc
            : throw new DomainRuleViolationException(
                "A data de criação do usuário deve estar em UTC.");
}
