using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Api.Middlewares;

internal static class ApiProblemDetailsFactory
{
    public static ValidationProblemDetails CreateValidation(
        HttpContext context,
        IDictionary<string, string[]> errors)
    {
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Um ou mais dados são inválidos",
            Detail = "Um ou mais dados informados são inválidos.",
            Instance = context.Request.Path
        };

        AddExtensions(
            context,
            problemDetails,
            "validation_error");

        return problemDetails;
    }

    public static void Customize(ProblemDetailsContext context)
    {
        var statusCode = context.ProblemDetails.Status ??
            context.HttpContext.Response.StatusCode;
        var hasApplicationCode =
            context.ProblemDetails.Extensions.ContainsKey("code");

        if (!hasApplicationCode)
        {
            context.ProblemDetails.Title = GetDefaultTitle(statusCode);
        }

        AddExtensions(
            context.HttpContext,
            context.ProblemDetails,
            GetDefaultCode(statusCode));
    }

    public static string GetTraceId(HttpContext context) =>
        Activity.Current?.Id ?? context.TraceIdentifier;

    private static void AddExtensions(
        HttpContext context,
        ProblemDetails problemDetails,
        string code)
    {
        problemDetails.Extensions.TryAdd("code", code);
        problemDetails.Extensions.TryAdd("traceId", GetTraceId(context));
    }

    private static string GetDefaultCode(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => "bad_request",
            StatusCodes.Status401Unauthorized => "authentication_required",
            StatusCodes.Status403Forbidden => "forbidden",
            StatusCodes.Status404NotFound => "not_found",
            StatusCodes.Status409Conflict => "conflict",
            StatusCodes.Status422UnprocessableEntity =>
                "business_rule_violation",
            StatusCodes.Status500InternalServerError => "internal_error",
            _ => "http_error"
        };

    private static string GetDefaultTitle(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => "Requisição inválida",
            StatusCodes.Status401Unauthorized => "Não autenticado",
            StatusCodes.Status403Forbidden => "Acesso não permitido",
            StatusCodes.Status404NotFound => "Recurso não encontrado",
            StatusCodes.Status409Conflict => "Conflito",
            StatusCodes.Status422UnprocessableEntity =>
                "Regra de negócio inválida",
            StatusCodes.Status500InternalServerError => "Erro interno",
            _ => "Erro HTTP"
        };
}
