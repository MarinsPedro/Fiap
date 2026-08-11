using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(
        HttpContext context,
        IProblemDetailsService problemDetailsService)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException)
            when (context.RequestAborted.IsCancellationRequested)
        {
            // A requisição foi cancelada pelo cliente.
        }
        catch (Exception exception)
            when (!context.Response.HasStarted)
        {
            var error = ResolveError(exception);

            LogException(context, exception, error);

            context.Response.Clear();
            context.Response.StatusCode = error.Status;

            await problemDetailsService.WriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails = CreateProblemDetails(
                        context,
                        error),
                    Exception = exception
                });
        }
    }

    private static ResolvedError ResolveError(Exception exception)
    {
        if (exception is DomainRuleViolationException domainException)
        {
            return new ResolvedError(
                StatusCodes.Status422UnprocessableEntity,
                "Regra de negócio inválida",
                "domain_rule_violation",
                domainException.Message);
        }

        if (exception is not AppException appException)
        {
            return InternalError();
        }

        return appException.Category switch
        {
            AppErrorCategory.Validation => new(
                StatusCodes.Status400BadRequest,
                "Um ou mais dados são inválidos",
                "validation_error",
                appException.Message,
                appException.Errors),

            AppErrorCategory.Authentication => new(
                StatusCodes.Status401Unauthorized,
                "Não autenticado",
                "authentication_error",
                appException.Message),

            AppErrorCategory.Forbidden => new(
                StatusCodes.Status403Forbidden,
                "Acesso não permitido",
                "forbidden",
                appException.Message),

            AppErrorCategory.NotFound => new(
                StatusCodes.Status404NotFound,
                "Recurso não encontrado",
                "not_found",
                appException.Message),

            AppErrorCategory.Conflict => new(
                StatusCodes.Status409Conflict,
                "Conflito",
                "conflict",
                appException.Message),

            AppErrorCategory.BusinessRule => new(
                StatusCodes.Status422UnprocessableEntity,
                "Regra de negócio inválida",
                "business_rule_violation",
                appException.Message),

            _ => InternalError()
        };
    }

    private static ResolvedError InternalError() =>
        new(
            StatusCodes.Status500InternalServerError,
            "Erro interno",
            "internal_error",
            "Ocorreu um erro interno inesperado.");

    private static ProblemDetails CreateProblemDetails(
        HttpContext context,
        ResolvedError error)
    {
        ProblemDetails problemDetails =
            error.Errors is { Count: > 0 }
                ? new ValidationProblemDetails(
                    error.Errors.ToDictionary(
                        item => item.Key,
                        item => item.Value,
                        StringComparer.OrdinalIgnoreCase))
                : new ProblemDetails();

        problemDetails.Status = error.Status;
        problemDetails.Title = error.Title;
        problemDetails.Detail = error.Detail;
        problemDetails.Instance = context.Request.Path.Value;
        problemDetails.Extensions["code"] = error.Code;
        problemDetails.Extensions["traceId"] =
            ApiProblemDetailsFactory.GetTraceId(context);

        return problemDetails;
    }

    private void LogException(
        HttpContext context,
        Exception exception,
        ResolvedError error)
    {
        if (error.Status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Erro não tratado ao processar {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            return;
        }

        logger.LogInformation(
            "Requisição rejeitada com {Code} ({Status}) em " +
            "{Method} {Path}: {Message}",
            error.Code,
            error.Status,
            context.Request.Method,
            context.Request.Path,
            error.Detail);
    }

    private sealed record ResolvedError(
        int Status,
        string Title,
        string Code,
        string Detail,
        IReadOnlyDictionary<string, string[]>? Errors = null);
}
