using FiapCloudGames.Domain.Common;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.UnitTests;

public sealed class UserTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void EmailShouldBeNormalized()
    {
        var email = Email.Create("  Aluno@FIAP.COM.BR ");

        Assert.Equal("aluno@fiap.com.br", email.Value);
    }

    [Fact]
    public void TryCreateShouldRejectInvalidEmailWithoutInvalidValueObject()
    {
        var created = Email.TryCreate(
            "email-invalido",
            out var email);

        Assert.False(created);
        Assert.Null(email);
    }

    [Fact]
    public void EmailShouldRejectValuesLongerThanMaximumLength()
    {
        var value =
            $"{new string('a', 243)}@fiap.com.br";

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => Email.Create(value));

        Assert.Equal(
            "O e-mail informado é inválido.",
            exception.Message);
    }

    [Theory]
    [InlineData("Ab1!")]
    [InlineData("abcdefgh!")]
    [InlineData("12345678!")]
    [InlineData("Abcdefgh1")]
    [InlineData("Abcdef1 ")]
    public void PasswordShouldRejectValueWithoutRequiredComposition(
        string value)
    {
        var created = Password.TryCreate(value, out var password);

        Assert.False(created);
        Assert.Null(password);
    }

    [Fact]
    public void PasswordShouldAcceptSecureValue()
    {
        const string value = "Senha@12";

        var password = Password.Create(value);

        Assert.Equal(value, password.Value);
        Assert.DoesNotContain(value, password.ToString());
    }

    [Fact]
    public void PasswordShouldReportTheBusinessRuleWhenInvalid()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => Password.Create("abcdefgh"));

        Assert.Equal(Password.InvalidMessage, exception.Message);
    }

    [Fact]
    public void CreateShouldNormalizeNameAndUseExplicitClock()
    {
        var user = User.Create(
            "  Aluno FIAP  ",
            Email.Create("aluno@fiap.com.br"),
            "hash-valido",
            CreatedAtUtc);

        Assert.Equal("Aluno FIAP", user.Name);
        Assert.Equal(CreatedAtUtc, user.CreatedAtUtc);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void ChangeNameShouldEnforceAggregateInvariant()
    {
        var user = User.Create(
            "Aluno FIAP",
            Email.Create("aluno@fiap.com.br"),
            "hash-valido",
            CreatedAtUtc);

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => user.ChangeName("A"));

        Assert.Equal(
            "O nome deve ter entre 2 e 120 caracteres.",
            exception.Message);
    }

    [Fact]
    public void DeactivateShouldMakeUserInactive()
    {
        var user = User.Create(
            "Aluno FIAP",
            Email.Create("aluno@fiap.com.br"),
            "hash-valido",
            CreatedAtUtc);

        user.Deactivate();

        Assert.False(user.IsActive);
    }
}
