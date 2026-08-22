using FiapCloudGames.Application.Common.Authentication;
using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Abstractions.Persistence;
using FiapCloudGames.Identity.Application.Features.Users.DeactivateUser;
using FiapCloudGames.Identity.Application.Features.Users.GetCurrentUser;
using FiapCloudGames.Identity.Application.Features.Users.GetUser;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Identity.Domain.Repositories;
using FiapCloudGames.Identity.Domain.ValueObjects;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Identity.UnitTests;

public sealed class UserApplicationServicesTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task GetCurrentShouldDelegateUsingAuthenticatedUserId()
    {
        var user = CreateUser();
        var users = new UserRepository(user);
        var getUser = new GetUserService(
            users,
            NullLogger<GetUserService>.Instance);
        var service = new GetCurrentUserService(
            new CurrentUserContext(user.Id),
            getUser);

        var result = await service.ExecuteAsync(
            CancellationToken.None);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Id, users.RequestedUserId);
    }

    [Fact]
    public async Task DeactivateInactiveUserShouldReturnConflictWithoutSaving()
    {
        var user = CreateUser();
        user.Deactivate();
        var users = new UserRepository(user);
        var unitOfWork = new IdentityUnitOfWork();
        var service = new DeactivateUserService(
            users,
            unitOfWork,
            NullLogger<DeactivateUserService>.Instance);

        var exception = await Assert.ThrowsAsync<AppException>(
            () => service.ExecuteAsync(
                user.Id,
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.Conflict, exception.Category);
        Assert.Equal("Usuário já desativado.", exception.Message);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    private static User CreateUser() =>
        User.Create(
            "Aluno FIAP",
            Email.Create("aluno@fiap.com.br"),
            "hash-valido",
            CreatedAtUtc);

    private sealed record CurrentUserContext(Guid? UserId) :
        ICurrentUserContext;

    private sealed class IdentityUnitOfWork : IIdentityUnitOfWork
    {
        public int SaveChangesCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            SaveChangesCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class UserRepository(User user) : IUserRepository
    {
        public Guid? RequestedUserId { get; private set; }

        public Task AddAsync(
            User userToAdd,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> ExistsAsync(
            Email email,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> ExistsEmailWithDifferentIdAsync(
            Guid id,
            Email email,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<User?> GetAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            RequestedUserId = id;
            return Task.FromResult<User?>(
                id == user.Id ? user : null);
        }

        public Task<User?> GetByEmailAsync(
            Email email,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
