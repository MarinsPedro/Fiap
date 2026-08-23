using FiapCloudGames.Domain.Common;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.UnitTests.Domain;

public sealed class PasswordTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Ab1!")]
    [InlineData("abcdefgh!")]
    [InlineData("12345678!")]
    [InlineData("Abcdefgh1")]
    [InlineData("Abcdef1 ")]
    public void TryCreate_WithoutRequiredComposition_ShouldReturnFalse(
        string? value)
    {
        var created = Password.TryCreate(value, out var password);

        Assert.False(created);
        Assert.Null(password);
    }

    [Fact]
    public void Create_AtMinimumLength_ShouldCreatePassword()
    {
        const string value = "Abcde1!?";

        var password = Password.Create(value);

        Assert.Equal(Password.MinimumLength, password.Value.Length);
        Assert.Equal(value, password.Value);
    }

    [Fact]
    public void Create_WithSecureValue_ShouldRedactStringRepresentation()
    {
        const string value = "Senha@12";

        var password = Password.Create(value);

        Assert.Equal("[REDACTED]", password.ToString());
        Assert.DoesNotContain(value, password.ToString());
    }

    [Fact]
    public void Create_WithInvalidValue_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => Password.Create("abcdefgh"));

        Assert.Equal(Password.InvalidMessage, exception.Message);
    }
}
