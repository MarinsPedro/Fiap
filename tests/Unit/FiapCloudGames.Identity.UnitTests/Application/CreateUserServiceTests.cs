using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Features.Users.CreateUser;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Identity.UnitTests.Application;

public sealed class CreateUserServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldCreateAndPersistUser()
    {
        var users = new FakeUserRepository();
        var unitOfWork = new SpyIdentityUnitOfWork();
        var passwordHasher = new StubPasswordHasher();
        var service = CreateService(users, unitOfWork, passwordHasher);

        var result = await service.ExecuteAsync(
            new CreateUserInput(
                "  Aluno FIAP  ",
                "  Aluno@FIAP.COM.BR  ",
                "Senha@12"),
            CancellationToken.None);

        var addedUser = Assert.IsType<Identity.Domain.Entities.User>(
            users.AddedUser);
        Assert.Equal(1, users.AddCalls);
        Assert.Equal("Aluno FIAP", addedUser.Name);
        Assert.Equal("aluno@fiap.com.br", addedUser.Email.Value);
        Assert.Equal("hash-gerado", addedUser.PasswordHash);
        Assert.Equal(IdentityTestData.NowUtc, addedUser.CreatedAtUtc);
        Assert.Equal("Senha@12", passwordHasher.HashedPassword?.Value);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
        Assert.Equal(addedUser.Id, result.Id);
        Assert.Equal("User", result.Role);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailAlreadyExists_ShouldThrowConflictWithoutPersisting()
    {
        var users = new FakeUserRepository { EmailExists = true };
        var unitOfWork = new SpyIdentityUnitOfWork();
        var passwordHasher = new StubPasswordHasher();
        var service = CreateService(users, unitOfWork, passwordHasher);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                new CreateUserInput(
                    "Aluno FIAP",
                    "aluno@fiap.com.br",
                    "Senha@12"),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.Conflict, exception.Category);
        Assert.Equal("Já existe um usuário com este e-mail.", exception.Message);
        Assert.Equal(0, passwordHasher.HashCalls);
        Assert.Null(users.AddedUser);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPassword_ShouldThrowValidationWithoutPersisting()
    {
        var users = new FakeUserRepository();
        var unitOfWork = new SpyIdentityUnitOfWork();
        var passwordHasher = new StubPasswordHasher();
        var service = CreateService(users, unitOfWork, passwordHasher);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                new CreateUserInput(
                    "Aluno FIAP",
                    "aluno@fiap.com.br",
                    "senha-fraca"),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.Validation, exception.Category);
        var error = Assert.Single(exception.Errors);
        Assert.Equal("password", error.Field);
        Assert.Equal(0, users.ExistsCalls);
        Assert.Equal(0, passwordHasher.HashCalls);
        Assert.Null(users.AddedUser);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    private static CreateUserService CreateService(
        FakeUserRepository users,
        SpyIdentityUnitOfWork unitOfWork,
        StubPasswordHasher passwordHasher) =>
        new(
            users,
            unitOfWork,
            passwordHasher,
            new FixedTimeProvider(IdentityTestData.NowUtc),
            NullLogger<CreateUserService>.Instance);
}
