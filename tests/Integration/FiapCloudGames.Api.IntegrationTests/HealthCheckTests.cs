namespace FiapCloudGames.Api.IntegrationTests;

public sealed class HealthCheckTests : IClassFixture<FiapCloudGamesApiFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(FiapCloudGamesApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task HealthEndpointShouldReturnSuccess()
    {
        var response = await _client.GetAsync(new Uri("/health", UriKind.Relative));

        response.EnsureSuccessStatusCode();
    }
}
