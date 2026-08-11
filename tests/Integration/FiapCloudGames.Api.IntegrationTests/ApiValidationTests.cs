using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FiapCloudGames.Api.IntegrationTests;

public sealed class ApiValidationTests :
    IClassFixture<FiapCloudGamesApiFactory>
{
    private readonly HttpClient _client;

    public ApiValidationTests(FiapCloudGamesApiFactory factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task InvalidJsonShouldReturnStandardValidationProblem()
    {
        using var content = new StringContent(
            "{ invalid-json",
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync(
            new Uri("/api/users", UriKind.Relative),
            content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStreamAsync());
        var root = document.RootElement;

        Assert.Equal(
            "validation_error",
            root.GetProperty("code").GetString());
        if (root.TryGetProperty("type", out var type))
        {
            Assert.DoesNotContain(
                "api.fiapcloudgames.com/errors",
                type.GetString() ?? string.Empty,
                StringComparison.OrdinalIgnoreCase);
        }
        Assert.True(root.GetProperty("errors").EnumerateObject().Any());
        Assert.False(
            string.IsNullOrWhiteSpace(
                root.GetProperty("traceId").GetString()));
    }

    [Fact]
    public async Task DataAnnotationsShouldRejectInvalidUserRequest()
    {
        var response = await _client.PostAsJsonAsync(
            new Uri("/api/users", UriKind.Relative),
            new
            {
                Name = "A",
                Email = "email-invalido",
                Password = "1234567"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStreamAsync());
        var root = document.RootElement;
        var errors = root.GetProperty("errors");

        Assert.Equal(
            "validation_error",
            root.GetProperty("code").GetString());
        Assert.True(errors.TryGetProperty("name", out _));
        Assert.True(errors.TryGetProperty("email", out _));
        Assert.True(errors.TryGetProperty("password", out _));
    }

    [Theory]
    [InlineData(
        "/rota-inexistente",
        HttpStatusCode.NotFound,
        "not_found",
        "Recurso não encontrado")]
    [InlineData(
        "/api/users/me",
        HttpStatusCode.Unauthorized,
        "authentication_required",
        "Não autenticado")]
    public async Task EmptyFrameworkErrorsShouldReturnProblemDetails(
        string path,
        HttpStatusCode expectedStatus,
        string expectedCode,
        string expectedTitle)
    {
        var response = await _client.GetAsync(
            new Uri(path, UriKind.Relative));

        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStreamAsync());
        var root = document.RootElement;

        Assert.Equal(expectedCode, root.GetProperty("code").GetString());
        Assert.Equal(expectedTitle, root.GetProperty("title").GetString());
        Assert.False(
            string.IsNullOrWhiteSpace(
                root.GetProperty("traceId").GetString()));
    }
}
