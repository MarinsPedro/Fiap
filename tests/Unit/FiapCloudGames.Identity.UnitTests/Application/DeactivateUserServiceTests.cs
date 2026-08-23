using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Features.Users.DeactivateUser;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Identity.UnitTests.Application;

public sealed class DeactivateUserServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithActiveUser_ShouldDeactivateAndPersist()
    {
        var user = IdentityTestData.CreateUser();
        var unitOfWork = new SpyIdentityUnitOfWork();
        var service = CreateService(
            new FakeUserRepository { User = user },
            unitOfWork);

        await service.ExecuteAsync(user.Id, CancellationToken.None);

        Assert.False(user.IsActive);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowNotFoundWithoutPersisting()
    {
        var unitOfWork = new SpyIdentityUnitOfWork();
        var service = CreateService(new FakeUserRepository(), unitOfWork);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Equal("Usuário não encontrado.", exception.Message);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsInactive_ShouldThrowConflictWithoutPersisting()
    {
        var user = IdentityTestData.CreateUser();
        user.Deactivate();
        var unitOfWork = new SpyIdentityUnitOfWork();
        var service = CreateService(
            new FakeUserRepository { User = user },
            unitOfWork);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(user.Id, CancellationToken.None));

        Assert.Equal(AppErrorCategory.Conflict, exception.Category);
        Assert.Equal("Usuário já desativado.", exception.Message);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    private static DeactivateUserService CreateService(
        FakeUserRepository users,
        SpyIdentityUnitOfWork unitOfWork) =>
        new(
            users,
            unitOfWork,
            NullLogger<DeactivateUserService>.Instance);
}
