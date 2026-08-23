using System.Net;
using System.Text.Json;
using FiapCloudGames.Presentation.Common.Errors;

namespace FiapCloudGames.Api.IntegrationTests.Support;

internal static class ApiProblemAssertions
{
    public static async Task<ApiProblemDetails> AssertAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        string expectedType,
        string expectedTitle,
        string expectedDetail,
        int? expectedErrorCount = null)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(
            ApiProblemDetailsContentTypes.Json,
            response.Content.Headers.ContentType?.MediaType);

        var payload = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ApiProblemDetails>(
            payload,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var typedProblem = Assert.IsType<ApiProblemDetails>(problem);
        using var document = JsonDocument.Parse(payload);

        Assert.Equal(expectedType, typedProblem.Type);
        Assert.Equal(expectedTitle, typedProblem.Title);
        Assert.Equal((int)expectedStatus, typedProblem.Status);
        Assert.Equal(expectedDetail, typedProblem.Detail);
        Assert.False(string.IsNullOrWhiteSpace(typedProblem.TraceId));
        Assert.False(document.RootElement.TryGetProperty("instance", out _));
        var hasErrors = document.RootElement.TryGetProperty(
            "errors",
            out var serializedErrors);

        if (expectedErrorCount is null)
        {
            Assert.False(hasErrors);
            Assert.Null(typedProblem.Errors);
            return typedProblem;
        }

        Assert.True(hasErrors);
        Assert.Equal(expectedErrorCount.Value, serializedErrors.GetArrayLength());
        Assert.Equal(expectedErrorCount.Value, typedProblem.Errors?.Count);

        var typedErrors = typedProblem.Errors!.ToArray();
        var jsonErrors = serializedErrors.EnumerateArray().ToArray();
        for (var index = 0; index < typedErrors.Length; index++)
        {
            Assert.False(string.IsNullOrWhiteSpace(typedErrors[index].Message));
            Assert.False(jsonErrors[index].TryGetProperty("code", out _));

            if (typedErrors[index].Field is null)
            {
                Assert.False(jsonErrors[index].TryGetProperty("field", out _));
            }
        }

        return typedProblem;
    }
}
