using System.Text.Json.Serialization;

namespace FiapCloudGames.Presentation.Common.Errors;

public sealed class ApiProblemDetails
{
    public required string Type { get; init; }

    public required string Title { get; init; }

    public required int Status { get; init; }

    public required string Detail { get; init; }

    public required string TraceId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyCollection<ApiError>? Errors { get; init; }
}
