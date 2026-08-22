using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace FiapCloudGames.Api.IntegrationTests;

public sealed class SwaggerProblemDetailsTests :
    IClassFixture<FiapCloudGamesApiFactory>
{
    private readonly FiapCloudGamesApiFactory _factory;

    public SwaggerProblemDetailsTests(FiapCloudGamesApiFactory factory) =>
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
                foreach (var response in operation.Value
                             .GetProperty("responses")
                             .EnumerateObject()
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
