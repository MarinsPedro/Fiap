namespace FiapCloudGames.Presentation.Common.Errors;

public static class ApiProblemTypes
{
    private const string Prefix = "urn:fiap-cloud-games:problem:";

    public const string Validation = Prefix + "validation";
    public const string BadRequest = Prefix + "bad-request";
    public const string Unauthorized = Prefix + "unauthorized";
    public const string Forbidden = Prefix + "forbidden";
    public const string NotFound = Prefix + "not-found";
    public const string Conflict = Prefix + "conflict";
    public const string BusinessRule = Prefix + "business-rule";
    public const string InternalServerError =
        Prefix + "internal-server-error";
    public const string HttpError = Prefix + "http-error";
}
