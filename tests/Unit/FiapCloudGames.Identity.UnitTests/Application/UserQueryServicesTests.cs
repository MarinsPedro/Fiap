using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Features.Users.FindUser;
using FiapCloudGames.Identity.Application.Features.Users.GetCurrentUser;
using FiapCloudGames.Identity.Application.Features.Users.GetUser;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Identity.UnitTests.Application;

public sealed class UserQueryServicesTests
{
    [Fact]
    public async Task FindUser_WithExistingUser_ShouldReturnMappedResult()
    {
        var user = IdentityTestData.CreateUser();
        var service = CreateFindService(
            new FakeUserRepository { User = user });

        var result = await service.ExecuteAsync(
            user.Id,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email.Value, result.Email);
        Assert.Equal("User", result.Role);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task FindUser_WhenUserDoesNotExist_ShouldReturnNull()
    {
        var service = CreateFindService(new FakeUserRepository());

        var result = await service.ExecuteAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUser_WhenUserDoesNotExist_ShouldThrowNotFound()
    {
        var service = new GetUserService(
            CreateFindService(new FakeUserRepository()));

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Equal("Usuário não encontrado.", exception.Message);
    }

    [Fact]
    public async Task GetCurrentUser_WithAuthenticatedUser_ShouldReturnThatUser()
    {
        var user = IdentityTestData.CreateUser();
        var users = new FakeUserRepository { User = user };
        var service = new GetCurrentUserService(
            new StubCurrentUserContext(user.Id),
            new GetUserService(CreateFindService(users)));

        var result = await service.ExecuteAsync(CancellationToken.None);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Id, users.RequestedUserId);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutAuthenticatedUser_ShouldThrowAuthentication()
    {
        var service = new GetCurrentUserService(
            new StubCurrentUserContext(null),
            new GetUserService(CreateFindService(new FakeUserRepository())));

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(CancellationToken.None));

        Assert.Equal(AppErrorCategory.Authentication, exception.Category);
        Assert.Equal(
            "O identificador do usuário autenticado é inválido.",
            exception.Message);
    }

    private static FindUserService CreateFindService(
        FakeUserRepository users) =>
        new(users, NullLogger<FindUserService>.Instance);
}
