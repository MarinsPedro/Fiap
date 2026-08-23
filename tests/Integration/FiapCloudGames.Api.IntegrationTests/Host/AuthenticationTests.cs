using System.Net;
using System.Net.Http.Headers;
using FiapCloudGames.Api.IntegrationTests.Support;
using FiapCloudGames.Presentation.Common.Errors;

namespace FiapCloudGames.Api.IntegrationTests.Host;

public sealed class AuthenticationTests :
    IClassFixture<FiapCloudGamesApiFactory>
{
    private readonly HttpClient _client;

    public AuthenticationTests(FiapCloudGamesApiFactory factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync(
            new Uri("/api/users/me", UriKind.Relative));

        await AssertUnauthorizedAsync(response);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithInvalidToken_ShouldReturnUnauthorized()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            new Uri("/api/users/me", UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            "token-invalido");

        var response = await _client.SendAsync(request);

        await AssertUnauthorizedAsync(response);
    }

    private static async Task AssertUnauthorizedAsync(HttpResponseMessage response)
    {
        await ApiProblemAssertions.AssertAsync(
            response,
            HttpStatusCode.Unauthorized,
            ApiProblemTypes.Unauthorized,
            "Não autenticado",
            "Token ausente, inválido ou expirado.");
    }
}
