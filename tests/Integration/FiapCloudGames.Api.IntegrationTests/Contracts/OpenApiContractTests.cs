using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

using FiapCloudGames.Api.IntegrationTests;

namespace FiapCloudGames.Api.IntegrationTests.Contracts;

public sealed class OpenApiContractTests :
    IClassFixture<FiapCloudGamesApiFactory>
{
    private readonly FiapCloudGamesApiFactory _factory;

    public OpenApiContractTests(FiapCloudGamesApiFactory factory) =>
        _factory = factory;

    [Fact]
    public async Task ErrorResponsesShouldExposeOnlyClosedApiProblemSchema()
    {
        using var developmentFactory = _factory.WithWebHostBuilder(
            builder => builder.UseEnvironment("Development"));
        using var client = developmentFactory.CreateClient();
        using var document = await JsonDocument.ParseAsync(
            await client.GetStreamAsync(
                new Uri("/swagger/v1/swagger.json", UriKind.Relative)));
        var root = document.RootElement;
        var schemas = root
            .GetProperty("components")
            .GetProperty("schemas");

        Assert.True(schemas.TryGetProperty("ApiProblemDetails", out var problem));
        Assert.True(schemas.TryGetProperty("ApiError", out var apiError));
        Assert.False(schemas.TryGetProperty("ProblemDetails", out _));
        Assert.False(schemas.TryGetProperty("ValidationProblemDetails", out _));

        AssertSchemaProperties(
            problem,
            "type",
            "title",
            "status",
            "detail",
            "traceId",
            "errors");
        AssertSchemaProperties(apiError, "message", "field");
        AssertRequiredProperties(
            problem,
            "type",
            "title",
            "status",
            "detail",
            "traceId");
        AssertRequiredProperties(apiError, "message");
        AssertClosedSchema(problem);
        AssertClosedSchema(apiError);

        foreach (var path in root.GetProperty("paths").EnumerateObject())
        {
            foreach (var operation in path.Value.EnumerateObject())
            {
                var responses = operation.Value.GetProperty("responses");
                Assert.True(
                    responses.TryGetProperty("500", out _),
                    $"{operation.Name.ToUpperInvariant()} {path.Name} não declara 500.");

                foreach (var response in responses.EnumerateObject()
                             .Where(item =>
                                 int.TryParse(item.Name, out var status) &&
                                 status >= 400))
                {
                    var content = response.Value.GetProperty("content");
                    var mediaType = Assert.Single(content.EnumerateObject());
                    Assert.Equal("application/problem+json", mediaType.Name);
                    Assert.Equal(
                        "#/components/schemas/ApiProblemDetails",
                        mediaType.Value
                            .GetProperty("schema")
                            .GetProperty("$ref")
                            .GetString());
                }
            }
        }
    }

    private static void AssertSchemaProperties(
        JsonElement schema,
        params string[] expectedProperties)
    {
        var actualProperties = schema
            .GetProperty("properties")
            .EnumerateObject()
            .Select(property => property.Name)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            expectedProperties.Order(StringComparer.Ordinal),
            actualProperties);
    }

    private static void AssertClosedSchema(JsonElement schema)
    {
        if (schema.TryGetProperty("additionalProperties", out var additional))
        {
            Assert.Equal(JsonValueKind.False, additional.ValueKind);
        }
    }

    private static void AssertRequiredProperties(
        JsonElement schema,
        params string[] expectedProperties)
    {
        var required = schema
            .GetProperty("required")
            .EnumerateArray()
            .Select(item => item.GetString())
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            expectedProperties.Order(StringComparer.Ordinal),
            required);
    }
}
