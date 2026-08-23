using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Features.Users.UpdateUser;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Identity.UnitTests.Application;

public sealed class UpdateUserServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldUpdateAndPersistUser()
    {
        var user = IdentityTestData.CreateUser();
        var users = new FakeUserRepository { User = user };
        var unitOfWork = new SpyIdentityUnitOfWork();
        var service = CreateService(user.Id, users, unitOfWork);

        var result = await service.ExecuteAsync(
            new UpdateUserInput("  Novo Nome  ", "NOVO@FIAP.COM.BR"),
            CancellationToken.None);

        Assert.Equal("Novo Nome", user.Name);
        Assert.Equal("novo@fiap.com.br", user.Email.Value);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailBelongsToAnotherUser_ShouldThrowConflictWithoutPersisting()
    {
        var user = IdentityTestData.CreateUser();
        var users = new FakeUserRepository
        {
            User = user,
            EmailExistsWithDifferentId = true
        };
        var unitOfWork = new SpyIdentityUnitOfWork();
        var service = CreateService(user.Id, users, unitOfWork);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                new UpdateUserInput("Novo Nome", "outro@fiap.com.br"),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.Conflict, exception.Category);
        Assert.Equal("Aluno FIAP", user.Name);
        Assert.Equal("aluno@fiap.com.br", user.Email.Value);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowNotFoundWithoutPersisting()
    {
        var unitOfWork = new SpyIdentityUnitOfWork();
        var service = CreateService(
            Guid.NewGuid(),
            new FakeUserRepository(),
            unitOfWork);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                new UpdateUserInput("Novo Nome", "novo@fiap.com.br"),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.NotFound, exception.Category);
        Assert.Equal("Usuário não encontrado.", exception.Message);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    private static UpdateUserService CreateService(
        Guid userId,
        FakeUserRepository users,
        SpyIdentityUnitOfWork unitOfWork) =>
        new(
            new StubCurrentUserContext(userId),
            users,
            unitOfWork,
            NullLogger<UpdateUserService>.Instance);
}
