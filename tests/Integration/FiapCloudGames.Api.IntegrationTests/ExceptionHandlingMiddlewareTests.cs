using System.Text.Json;
using FiapCloudGames.Api.Middlewares;
using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Domain.Common;
using FiapCloudGames.Presentation.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Api.IntegrationTests;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Theory]
    [InlineData(
        "validation",
        400,
        ApiProblemTypes.Validation,
        AppErrorCategory.Validation)]
    [InlineData(
        "authentication",
        401,
        ApiProblemTypes.Unauthorized,
        AppErrorCategory.Authentication)]
    [InlineData(
        "forbidden",
        403,
        ApiProblemTypes.Forbidden,
        AppErrorCategory.Forbidden)]
    [InlineData(
        "not-found",
        404,
        ApiProblemTypes.NotFound,
        AppErrorCategory.NotFound)]
    [InlineData(
        "conflict",
        409,
        ApiProblemTypes.Conflict,
        AppErrorCategory.Conflict)]
    [InlineData(
        "business-rule",
        422,
        ApiProblemTypes.BusinessRule,
        AppErrorCategory.BusinessRule)]
    public async Task ShouldMapSemanticApplicationExceptions(
        string exceptionKind,
        int expectedStatus,
        string expectedType,
        AppErrorCategory expectedCategory)
    {
        var exception = CreateSemanticException(exceptionKind);
        var result = await InvokeAsync(exception);
        var problem = result.Problem;

        Assert.Equal(expectedCategory, exception.Category);
        Assert.Equal(expectedStatus, result.StatusCode);
        Assert.Equal(ApiProblemDetailsContentTypes.Json, result.ContentType);
        Assert.Equal(expectedType, problem.Type);
        Assert.Equal(expectedStatus, problem.Status);
        Assert.False(string.IsNullOrWhiteSpace(problem.Title));
        Assert.False(string.IsNullOrWhiteSpace(problem.Detail));
        Assert.False(string.IsNullOrWhiteSpace(problem.TraceId));
        Assert.Equal(exception.Message, problem.Detail);
        Assert.Null(problem.Errors);
    }

    [Theory]
    [InlineData("invalid-operation")]
    [InlineData("key-not-found")]
    [InlineData("argument")]
    [InlineData("unauthorized-access")]
    public async Task GenericExceptionsShouldRemainSafeInternalErrors(
        string exceptionKind)
    {
        var exception = CreateGenericException(exceptionKind);
        var result = await InvokeAsync(exception);
        var problem = result.Problem;

        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Equal(ApiProblemTypes.InternalServerError, problem.Type);
        Assert.Equal("Erro interno", problem.Title);
        Assert.Equal("Não foi possível concluir a operação.", problem.Detail);
        Assert.Null(problem.Errors);
        Assert.DoesNotContain(
            exception.Message,
            JsonSerializer.Serialize(problem),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task DomainRuleViolationsShouldReturnBusinessRuleError()
    {
        var exception = new DomainRuleViolationException(
            "O fim da promoção deve ser posterior ao início.");
        var result = await InvokeAsync(exception);
        var problem = result.Problem;

        Assert.Equal(
            StatusCodes.Status422UnprocessableEntity,
            result.StatusCode);
        Assert.Equal(ApiProblemTypes.BusinessRule, problem.Type);
        Assert.Equal("Regra de negócio não atendida", problem.Title);
        Assert.Equal(exception.Message, problem.Detail);
        Assert.Null(problem.Errors);
    }

    [Fact]
    public async Task ValidationShouldPreserveMultipleFieldErrors()
    {
        var exception = AppException.Validation(
            [
                new AppError(
                    "O nome é obrigatório.",
                    "name"),
                new AppError(
                    "O e-mail é inválido.",
                    "email")
            ]);

        var result = await InvokeAsync(exception);
        var problem = result.Problem;

        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal(ApiProblemTypes.Validation, problem.Type);
        var errors = Assert.IsAssignableFrom<IReadOnlyCollection<ApiError>>(
            problem.Errors);
        Assert.Collection(
            errors,
            error =>
            {
                Assert.Equal("O nome é obrigatório.", error.Message);
                Assert.Equal("name", error.Field);
            },
            error =>
            {
                Assert.Equal("O e-mail é inválido.", error.Message);
                Assert.Equal("email", error.Field);
            });
    }

    [Fact]
    public async Task ClientCancellationShouldNotBecomeInternalError()
    {
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        var context = new DefaultHttpContext
        {
            RequestAborted = cancellationSource.Token
        };

        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => Task.FromCanceled(cancellationSource.Token),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.Equal(0, context.Response.Body.Length);
    }

    private static AppException CreateSemanticException(string kind) =>
        kind switch
        {
            "validation" => AppException.Validation(
                "A requisição é inválida."),
            "authentication" => AppException.Authentication(
                "E-mail ou senha inválidos."),
            "forbidden" => AppException.Forbidden(
                "O usuário está inativo."),
            "not-found" => AppException.NotFound(
                "Jogo não encontrado."),
            "conflict" => AppException.Conflict(
                "Jogo já está desativado."),
            "business-rule" => AppException.BusinessRule(
                "O usuário está inativo."),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };

    private static Exception CreateGenericException(string kind) =>
        kind switch
        {
            "invalid-operation" => new InvalidOperationException(
                "A connection string não foi configurada."),
            "key-not-found" => new KeyNotFoundException(
                "A chave interna não existe."),
            "argument" => new ArgumentException(
                "Um parâmetro interno é inválido."),
            "unauthorized-access" => new UnauthorizedAccessException(
                "Acesso interno negado."),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };

    private static async Task<MiddlewareResult> InvokeAsync(
        Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/tests/errors";
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => Task.FromException(exception),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        var problem = await JsonSerializer.DeserializeAsync<ApiProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        return new MiddlewareResult(
            context.Response.StatusCode,
            context.Response.ContentType,
            Assert.IsType<ApiProblemDetails>(problem));
    }

    private sealed record MiddlewareResult(
        int StatusCode,
        string? ContentType,
        ApiProblemDetails Problem);
}
