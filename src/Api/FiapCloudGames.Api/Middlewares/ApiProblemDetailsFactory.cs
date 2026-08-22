using System.Diagnostics;
using FiapCloudGames.Presentation.Common.Errors;

namespace FiapCloudGames.Api.Middlewares;

internal static class ApiProblemDetailsFactory
{
    private static readonly object CurrentProblemDetailsKey = new();

    public static ApiProblemDetails CreateValidation(
        HttpContext context,
        IReadOnlyCollection<ApiError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        return Create(
            context,
            ApiProblemDescriptors.Validation,
            errors: errors);
    }

    public static ApiProblemDetails Create(
        HttpContext context,
        ApiProblemDescriptor descriptor,
        string? detail = null,
        IReadOnlyCollection<ApiError>? errors = null)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(descriptor);

        var resolvedDetail = detail ?? descriptor.DefaultDetail;
        ArgumentException.ThrowIfNullOrWhiteSpace(resolvedDetail);

        if (errors is { Count: 0 })
        {
            throw new ArgumentException(
                "Quando informada, a coleção deve possuir pelo menos um erro.",
                nameof(errors));
        }

        if (errors?.Any(error =>
                error is null ||
                string.IsNullOrWhiteSpace(error.Message)) == true)
        {
            throw new ArgumentException(
                "Todos os erros devem possuir uma mensagem.",
                nameof(errors));
        }

        var problemDetails = new ApiProblemDetails
        {
            Type = descriptor.Type,
            Title = descriptor.Title,
            Status = descriptor.Status,
            Detail = resolvedDetail,
            TraceId = GetTraceId(context),
            Errors = errors?.ToArray()
        };

        context.Items[CurrentProblemDetailsKey] = problemDetails;
        return problemDetails;
    }

    public static ApiProblemDetails CreateStatusCode(
        HttpContext context,
        int status)
    {
        var descriptor = ApiProblemDescriptors.FromStatusCode(status);

        return Create(context, descriptor);
    }

    public static string GetTraceId(HttpContext context) =>
        Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

    public static ApiProblemDetails? GetCurrent(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Items.TryGetValue(
            CurrentProblemDetailsKey,
            out var problemDetails)
                ? problemDetails as ApiProblemDetails
                : null;
    }
}
