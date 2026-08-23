using FiapCloudGames.Application.Common.Authentication;
using FiapCloudGames.Identity.Application.Abstractions.Persistence;
using FiapCloudGames.Identity.Application.Abstractions.Security;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Identity.Domain.Repositories;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.UnitTests.Application;

internal static class IdentityTestData
{
    internal static readonly DateTimeOffset NowUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    internal static User CreateUser(string email = "aluno@fiap.com.br") =>
        User.Create(
            "Aluno FIAP",
            Email.Create(email),
            "hash-valido",
            NowUtc);
}

internal sealed class FakeUserRepository : IUserRepository
{
    internal User? User { get; set; }
    internal User? UserByEmail { get; set; }
    internal User? AddedUser { get; private set; }
    internal bool EmailExists { get; set; }
    internal bool EmailExistsWithDifferentId { get; set; }
    internal int AddCalls { get; private set; }
    internal int ExistsCalls { get; private set; }
    internal Guid? RequestedUserId { get; private set; }
    internal Email? RequestedEmail { get; private set; }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        AddCalls++;
        AddedUser = user;
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(
        Email email,
        CancellationToken cancellationToken)
    {
        ExistsCalls++;
        RequestedEmail = email;
        return Task.FromResult(EmailExists);
    }

    public Task<bool> ExistsEmailWithDifferentIdAsync(
        Guid id,
        Email email,
        CancellationToken cancellationToken)
    {
        RequestedUserId = id;
        RequestedEmail = email;
        return Task.FromResult(EmailExistsWithDifferentId);
    }

    public Task<User?> GetAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        RequestedUserId = id;
        return Task.FromResult(
            User?.Id == id
                ? User
                : null);
    }

    public Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken)
    {
        RequestedEmail = email;
        return Task.FromResult(
            UserByEmail?.Email == email
                ? UserByEmail
                : null);
    }
}

internal sealed class SpyIdentityUnitOfWork : IIdentityUnitOfWork
{
    internal int SaveChangesCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}

internal sealed class StubPasswordHasher : IPasswordHasher
{
    internal string HashResult { get; set; } = "hash-gerado";
    internal bool VerifyResult { get; set; }
    internal Password? HashedPassword { get; private set; }
    internal string? VerifiedPassword { get; private set; }
    internal int HashCalls { get; private set; }
    internal int VerifyCalls { get; private set; }

    public string Hash(Password password)
    {
        HashCalls++;
        HashedPassword = password;
        return HashResult;
    }

    public bool Verify(string password, string passwordHash)
    {
        VerifyCalls++;
        VerifiedPassword = password;
        return VerifyResult;
    }
}

internal sealed class StubTokenGenerator : ITokenGenerator
{
    internal GeneratedToken Token { get; set; } =
        new("token-gerado", IdentityTestData.NowUtc.AddHours(2));
    internal User? GeneratedFor { get; private set; }
    internal int GenerateCalls { get; private set; }

    public GeneratedToken Generate(User user)
    {
        GenerateCalls++;
        GeneratedFor = user;
        return Token;
    }
}

internal sealed record StubCurrentUserContext(Guid? UserId) :
    ICurrentUserContext;

internal sealed class FixedTimeProvider(DateTimeOffset nowUtc) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => nowUtc;
}
