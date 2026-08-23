using System.Net;
using System.Net.Http.Headers;
using FiapCloudGames.Api.IntegrationTests.Support;
using FiapCloudGames.Presentation.Common.Errors;

namespace FiapCloudGames.Api.IntegrationTests.Host;

public sealed class AuthorizationTests :
    IClassFixture<FiapCloudGamesApiFactory>
{
    private readonly HttpClient _client;

    public AuthorizationTests(FiapCloudGamesApiFactory factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task AdminEndpoint_WithInsufficientRole_ShouldReturnForbidden()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            new Uri($"/api/users/{Guid.NewGuid()}", UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            TestJwtTokenFactory.Create("User"));

        var response = await _client.SendAsync(request);

        await ApiProblemAssertions.AssertAsync(
            response,
            HttpStatusCode.Forbidden,
            ApiProblemTypes.Forbidden,
            "Acesso negado",
            "O usuário autenticado não possui a permissão necessária.");
    }
}
