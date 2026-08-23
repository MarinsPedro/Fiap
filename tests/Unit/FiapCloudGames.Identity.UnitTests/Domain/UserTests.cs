using FiapCloudGames.Domain.Common;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Identity.Domain.Enums;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.UnitTests.Domain;

public sealed class UserTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_ShouldCreateActiveUser()
    {
        var user = User.Create(
            "  Aluno FIAP  ",
            Email.Create("aluno@fiap.com.br"),
            "  hash-valido  ",
            CreatedAtUtc);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Aluno FIAP", user.Name);
        Assert.Equal("aluno@fiap.com.br", user.Email.Value);
        Assert.Equal("hash-valido", user.PasswordHash);
        Assert.Equal(UserRole.User, user.Role);
        Assert.True(user.IsActive);
        Assert.Equal(CreatedAtUtc, user.CreatedAtUtc);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(120)]
    public void Create_WithNameAtValidBoundary_ShouldCreateUser(int length)
    {
        var user = User.Create(
            new string('a', length),
            Email.Create("aluno@fiap.com.br"),
            "hash-valido",
            CreatedAtUtc);

        Assert.Equal(length, user.Name.Length);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(121)]
    public void Create_WithNameOutsideBoundary_ShouldThrowBusinessRule(
        int length)
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            User.Create(
                new string('a', length),
                Email.Create("aluno@fiap.com.br"),
                "hash-valido",
                CreatedAtUtc));

        Assert.Equal(
            "O nome deve ter entre 2 e 120 caracteres.",
            exception.Message);
    }

    [Fact]
    public void Create_WithoutEmail_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            User.Create(
                "Aluno FIAP",
                null!,
                "hash-valido",
                CreatedAtUtc));

        Assert.Equal("O e-mail do usuário é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public void Create_WithInvalidRole_ShouldThrowBusinessRule(int role)
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            User.Create(
                "Aluno FIAP",
                Email.Create("aluno@fiap.com.br"),
                "hash-valido",
                CreatedAtUtc,
                (UserRole)role));

        Assert.Equal("O perfil do usuário é inválido.", exception.Message);
    }

    [Fact]
    public void Create_WithoutPasswordHash_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            User.Create(
                "Aluno FIAP",
                Email.Create("aluno@fiap.com.br"),
                " ",
                CreatedAtUtc));

        Assert.Equal("O hash da senha é obrigatório.", exception.Message);
    }

    [Fact]
    public void Create_WithDefaultCreationDate_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            User.Create(
                "Aluno FIAP",
                Email.Create("aluno@fiap.com.br"),
                "hash-valido",
                default));

        Assert.Equal(
            "A data de criação do usuário deve estar em UTC.",
            exception.Message);
    }

    [Fact]
    public void Create_WithNonUtcCreationDate_ShouldThrowBusinessRule()
    {
        var nonUtc = CreatedAtUtc.ToOffset(TimeSpan.FromHours(-3));

        var exception = Assert.Throws<DomainRuleViolationException>(() =>
            User.Create(
                "Aluno FIAP",
                Email.Create("aluno@fiap.com.br"),
                "hash-valido",
                nonUtc));

        Assert.Equal(
            "A data de criação do usuário deve estar em UTC.",
            exception.Message);
    }

    [Fact]
    public void ChangeDetails_WithValidData_ShouldUpdateNameAndEmail()
    {
        var user = CreateUser();

        user.ChangeDetails(
            "  Novo Nome  ",
            Email.Create("novo@fiap.com.br"));

        Assert.Equal("Novo Nome", user.Name);
        Assert.Equal("novo@fiap.com.br", user.Email.Value);
    }

    [Fact]
    public void Deactivate_WhenCalledMoreThanOnce_ShouldRemainInactive()
    {
        var user = CreateUser();

        user.Deactivate();
        user.Deactivate();

        Assert.False(user.IsActive);
    }

    private static User CreateUser() =>
        User.Create(
            "Aluno FIAP",
            Email.Create("aluno@fiap.com.br"),
            "hash-valido",
            CreatedAtUtc);
}
