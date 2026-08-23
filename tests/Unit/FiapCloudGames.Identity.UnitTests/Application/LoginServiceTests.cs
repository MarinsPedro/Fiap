using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Identity.Application.Features.Authentication.Login;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Identity.UnitTests.Application;

public sealed class LoginServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ShouldReturnGeneratedTokenAndUser()
    {
        var user = IdentityTestData.CreateUser();
        var passwordHasher = new StubPasswordHasher { VerifyResult = true };
        var tokenGenerator = new StubTokenGenerator();
        var service = CreateService(
            new FakeUserRepository { UserByEmail = user },
            passwordHasher,
            tokenGenerator);

        var result = await service.ExecuteAsync(
            new LoginInput("ALUNO@FIAP.COM.BR", "Senha@12"),
            CancellationToken.None);

        Assert.Equal(tokenGenerator.Token.AccessToken, result.AccessToken);
        Assert.Equal(tokenGenerator.Token.ExpiresAtUtc, result.ExpiresAtUtc);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal("Senha@12", passwordHasher.VerifiedPassword);
        Assert.Same(user, tokenGenerator.GeneratedFor);
    }

    [Theory]
    [InlineData("email-invalido", "Senha@12")]
    [InlineData("aluno@fiap.com.br", " ")]
    public async Task ExecuteAsync_WithMalformedCredentials_ShouldThrowAuthentication(
        string email,
        string password)
    {
        var passwordHasher = new StubPasswordHasher();
        var tokenGenerator = new StubTokenGenerator();
        var service = CreateService(
            new FakeUserRepository(),
            passwordHasher,
            tokenGenerator);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                new LoginInput(email, password),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.Authentication, exception.Category);
        Assert.Equal("E-mail ou senha inválidos.", exception.Message);
        Assert.Equal(0, passwordHasher.VerifyCalls);
        Assert.Equal(0, tokenGenerator.GenerateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowAuthentication()
    {
        var passwordHasher = new StubPasswordHasher();
        var tokenGenerator = new StubTokenGenerator();
        var service = CreateService(
            new FakeUserRepository(),
            passwordHasher,
            tokenGenerator);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                new LoginInput("aluno@fiap.com.br", "Senha@12"),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.Authentication, exception.Category);
        Assert.Equal(0, passwordHasher.VerifyCalls);
        Assert.Equal(0, tokenGenerator.GenerateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_WithWrongPassword_ShouldThrowAuthenticationWithoutGeneratingToken()
    {
        var user = IdentityTestData.CreateUser();
        var passwordHasher = new StubPasswordHasher { VerifyResult = false };
        var tokenGenerator = new StubTokenGenerator();
        var service = CreateService(
            new FakeUserRepository { UserByEmail = user },
            passwordHasher,
            tokenGenerator);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                new LoginInput("aluno@fiap.com.br", "Senha@99"),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.Authentication, exception.Category);
        Assert.Equal(1, passwordHasher.VerifyCalls);
        Assert.Equal(0, tokenGenerator.GenerateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsInactive_ShouldThrowForbiddenWithoutGeneratingToken()
    {
        var user = IdentityTestData.CreateUser();
        user.Deactivate();
        var tokenGenerator = new StubTokenGenerator();
        var service = CreateService(
            new FakeUserRepository { UserByEmail = user },
            new StubPasswordHasher { VerifyResult = true },
            tokenGenerator);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                new LoginInput("aluno@fiap.com.br", "Senha@12"),
                CancellationToken.None));

        Assert.Equal(AppErrorCategory.Forbidden, exception.Category);
        Assert.Equal("O usuário está inativo.", exception.Message);
        Assert.Equal(0, tokenGenerator.GenerateCalls);
    }

    private static LoginService CreateService(
        FakeUserRepository users,
        StubPasswordHasher passwordHasher,
        StubTokenGenerator tokenGenerator) =>
        new(
            users,
            passwordHasher,
            tokenGenerator,
            NullLogger<LoginService>.Instance);
}
