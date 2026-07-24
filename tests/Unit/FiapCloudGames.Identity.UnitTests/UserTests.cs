using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Identity.Domain.ValueObjects;

namespace FiapCloudGames.Identity.UnitTests;

public sealed class UserTests
{
    [Fact]
    public void EmailShouldBeNormalized()
    {
        var email = Email.Create("  Aluno@FIAP.COM.BR ");

        Assert.Equal("aluno@fiap.com.br", email.Value);
    }

    [Fact]
    public void DeactivateShouldMakeUserInactive()
    {
        var user = User.Create("Aluno FIAP", Email.Create("aluno@fiap.com.br"), "hash-valido");

        user.Deactivate();

        Assert.False(user.IsActive);
    }
}
