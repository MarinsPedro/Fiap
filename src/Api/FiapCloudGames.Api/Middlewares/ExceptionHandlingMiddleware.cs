using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IProblemDetailsService problemDetailsService)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            var (status, title) = Map(exception);
            if (status >= StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "Falha não tratada ao processar {Method} {Path}", context.Request.Method, context.Request.Path);
            }
            else
            {
                logger.LogWarning("Requisição rejeitada com status {Status}: {Message}", status, exception.Message);
            }

            context.Response.StatusCode = status;
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = status < 500 ? exception.Message : "Ocorreu um erro interno inesperado.",
                    Instance = context.Request.Path
                },
                Exception = exception
            });
        }
    }

    private static (int Status, string Title) Map(Exception exception) => exception switch
    {
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Não autenticado"),
        KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
        ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
        InvalidOperationException => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio inválida"),
        _ => (StatusCodes.Status500InternalServerError, "Erro interno")
    };
}
