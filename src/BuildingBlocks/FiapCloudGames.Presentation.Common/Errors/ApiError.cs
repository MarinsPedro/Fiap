using System.Text.Json.Serialization;

namespace FiapCloudGames.Presentation.Common.Errors;

public sealed class ApiError
{
    public required string Message { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Field { get; init; }
}
