using FiapCloudGames.Api.Configuration;

namespace FiapCloudGames.Api.IntegrationTests;

public sealed class ApiBehaviorOptionsExtensionsTests
{
    [Theory]
    [InlineData("Address.PostalCode", "address.postalCode")]
    [InlineData("$.Address.PostalCode", "address.postalCode")]
    [InlineData("Items[0].GameId", "items[0].gameId")]
    [InlineData("$[0].GameId", "[0].gameId")]
    [InlineData("Name", "name")]
    [InlineData("$", null)]
    [InlineData("", null)]
    public void ShouldNormalizeEveryFieldPathSegment(
        string fieldName,
        string? expected)
    {
        var result = ApiBehaviorOptionsExtensions.NormalizeFieldName(
            fieldName);

        Assert.Equal(expected, result);
    }
}
