using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Domain.Common;
using FiapCloudGames.Presentation.Common.Errors;

namespace FiapCloudGames.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException)
            when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogDebug(
                "Requisição cancelada pelo cliente durante {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);
        }
        catch (Exception exception)
            when (!context.Response.HasStarted)
        {
            var error = ResolveError(exception);

            LogUnexpectedException(context, exception, error);

            context.Response.Clear();
            context.Response.StatusCode = error.Descriptor.Status;

            var problemDetails = ApiProblemDetailsFactory.Create(
                context,
                error.Descriptor,
                error.Detail,
                error.Errors);

            await context.Response.WriteAsJsonAsync(
                problemDetails,
                options: null,
                contentType: ApiProblemDetailsContentTypes.Json,
                cancellationToken: context.RequestAborted);
        }
    }

    private static ResolvedError ResolveError(Exception exception)
    {
        if (exception is DomainRuleViolationException domainException)
        {
            return new ResolvedError(
                ApiProblemDescriptors.BusinessRule,
                domainException.Message);
        }

        if (exception is not AppException appException)
        {
            return InternalError();
        }

        var descriptor = ApiProblemDescriptors.FromCategory(
            appException.Category);

        var errors = appException.HasErrors
            ? appException.Errors
                .Select(error => new ApiError
                {
                    Message = error.Message,
                    Field = error.Field
                })
                .ToArray()
            : null;

        var detail = appException.HasErrors
            ? "Verifique os dados informados."
            : appException.Message;

        return new ResolvedError(
            descriptor,
            detail,
            errors);
    }

    private static ResolvedError InternalError() =>
        new(
            ApiProblemDescriptors.InternalServerError,
            ApiProblemDescriptors.InternalServerError.DefaultDetail);

    private void LogUnexpectedException(
        HttpContext context,
        Exception exception,
        ResolvedError error)
    {
        if (error.Descriptor.Status <
            StatusCodes.Status500InternalServerError)
        {
            return;
        }

        logger.LogError(
            exception,
            "Erro não tratado ao processar {Method} {Path}.",
            context.Request.Method,
            context.Request.Path);
    }

    private sealed record ResolvedError(
        ApiProblemDescriptor Descriptor,
        string Detail,
        IReadOnlyCollection<ApiError>? Errors = null);
}
