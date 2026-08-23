using FiapCloudGames.Domain.Common;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.UnitTests.Domain;

public sealed class EmailTests
{
    [Fact]
    public void Create_WithMixedCaseAndSpaces_ShouldNormalizeValue()
    {
        var email = Email.Create("  Aluno@FIAP.COM.BR ");

        Assert.Equal("aluno@fiap.com.br", email.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("email-invalido")]
    [InlineData("@fiap.com.br")]
    public void TryCreate_WithInvalidValue_ShouldReturnFalse(string? value)
    {
        var created = Email.TryCreate(value, out var email);

        Assert.False(created);
        Assert.Null(email);
    }

    [Fact]
    public void Create_WithMaximumLength_ShouldCreateEmail()
    {
        var value = $"{new string('a', 242)}@fiap.com.br";

        var email = Email.Create(value);

        Assert.Equal(254, email.Value.Length);
    }

    [Fact]
    public void Create_AboveMaximumLength_ShouldThrowBusinessRule()
    {
        var value = $"{new string('a', 243)}@fiap.com.br";

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => Email.Create(value));

        Assert.Equal("O e-mail informado é inválido.", exception.Message);
    }
}
