namespace FiapCloudGames.Application.Common.Exceptions;

public sealed class AppException : Exception
{
    private AppException(
        AppErrorCategory category,
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Category = category;
        Errors = errors?.ToDictionary(
            item => item.Key,
            item => item.Value.ToArray(),
            StringComparer.OrdinalIgnoreCase);
    }

    public AppErrorCategory Category { get; }

    public IReadOnlyDictionary<string, string[]>? Errors { get; }

    public bool HasErrors => Errors is { Count: > 0 };

    public static AppException Validation(string message) =>
        new(
            AppErrorCategory.Validation,
            message);

    public static AppException Validation(
        IReadOnlyDictionary<string, string[]> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException(
                "Informe pelo menos um erro de validação.",
                nameof(errors));
        }

        return new AppException(
            AppErrorCategory.Validation,
            "Um ou mais dados informados são inválidos.",
            errors);
    }

    public static AppException Authentication(string message) =>
        new(
            AppErrorCategory.Authentication,
            message);

    public static AppException Forbidden(string message) =>
        new(
            AppErrorCategory.Forbidden,
            message);

    public static AppException NotFound(string message) =>
        new(
            AppErrorCategory.NotFound,
            message);

    public static AppException Conflict(string message) =>
        new(
            AppErrorCategory.Conflict,
            message);

    public static AppException BusinessRule(string message) =>
        new(
            AppErrorCategory.BusinessRule,
            message);
}
