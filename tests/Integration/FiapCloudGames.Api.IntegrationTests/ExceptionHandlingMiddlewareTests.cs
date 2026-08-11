using System.Text.Json;
using FiapCloudGames.Api.Middlewares;
using FiapCloudGames.Application.Common.Exceptions;
using FiapCloudGames.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Api.IntegrationTests;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Theory]
    [InlineData(
        "validation",
        400,
        "validation_error",
        AppErrorCategory.Validation)]
    [InlineData(
        "authentication",
        401,
        "authentication_error",
        AppErrorCategory.Authentication)]
    [InlineData(
        "forbidden",
        403,
        "forbidden",
        AppErrorCategory.Forbidden)]
    [InlineData(
        "not-found",
        404,
        "not_found",
        AppErrorCategory.NotFound)]
    [InlineData(
        "conflict",
        409,
        "conflict",
        AppErrorCategory.Conflict)]
    [InlineData(
        "business-rule",
        422,
        "business_rule_violation",
        AppErrorCategory.BusinessRule)]
    public async Task ShouldMapOnlySemanticApplicationExceptions(
        string exceptionKind,
        int expectedStatus,
        string expectedCode,
        AppErrorCategory expectedCategory)
    {
        var exception = CreateSemanticException(exceptionKind);
        var result = await InvokeAsync(exception);
        using var document = result.Document;

        Assert.Equal(expectedCategory, exception.Category);
        Assert.Equal(expectedStatus, result.StatusCode);
        Assert.Equal(
            expectedStatus,
            document.RootElement.GetProperty("status").GetInt32());
        Assert.Equal(
            expectedCode,
            document.RootElement.GetProperty("code").GetString());
        Assert.False(
            string.IsNullOrWhiteSpace(
                document.RootElement.GetProperty("traceId").GetString()));
        Assert.Equal(
            exception.Message,
            document.RootElement.GetProperty("detail").GetString());
        Assert.DoesNotContain(
            "api.fiapcloudgames.com/errors",
            document.RootElement.GetProperty("type").GetString() ??
                string.Empty,
            StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("invalid-operation")]
    [InlineData("key-not-found")]
    [InlineData("argument")]
    [InlineData("unauthorized-access")]
    public async Task GenericExceptionsShouldRemainInternalServerErrors(
        string exceptionKind)
    {
        var exception = CreateGenericException(exceptionKind);
        var result = await InvokeAsync(exception);
        using var document = result.Document;

        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Equal(
            "internal_error",
            document.RootElement.GetProperty("code").GetString());
        Assert.Equal(
            "Ocorreu um erro interno inesperado.",
            document.RootElement.GetProperty("detail").GetString());
        Assert.DoesNotContain(
            exception.Message,
            document.RootElement.GetRawText(),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task DomainRuleViolationsShouldReturnUnprocessableEntity()
    {
        var exception = new DomainRuleViolationException(
            "O fim da promoção deve ser posterior ao início.");
        var result = await InvokeAsync(exception);
        using var document = result.Document;
        var root = document.RootElement;

        Assert.Equal(
            StatusCodes.Status422UnprocessableEntity,
            result.StatusCode);
        Assert.Equal(
            "Regra de negócio inválida",
            root.GetProperty("title").GetString());
        Assert.Equal(
            "domain_rule_violation",
            root.GetProperty("code").GetString());
        Assert.Equal(
            exception.Message,
            root.GetProperty("detail").GetString());
        Assert.False(
            string.IsNullOrWhiteSpace(
                root.GetProperty("traceId").GetString()));
    }

    [Fact]
    public async Task ValidationCategoryShouldReturnValidationProblemDetails()
    {
        var exception = AppException.Validation(
            new Dictionary<string, string[]>
            {
                ["name"] = ["O nome é obrigatório."],
                ["email"] =
                [
                    "O e-mail é inválido.",
                    "O e-mail já está cadastrado."
                ]
            });

        var result = await InvokeAsync(exception);
        using var document = result.Document;
        var root = document.RootElement;

        Assert.Equal(AppErrorCategory.Validation, exception.Category);
        Assert.True(exception.HasErrors);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("validation_error", root.GetProperty("code").GetString());
        Assert.Equal(
            "Um ou mais dados informados são inválidos.",
            root.GetProperty("detail").GetString());
        Assert.DoesNotContain(
            "api.fiapcloudgames.com/errors",
            root.GetProperty("type").GetString() ?? string.Empty,
            StringComparison.OrdinalIgnoreCase);
        Assert.Equal(
            1,
            root.GetProperty("errors").GetProperty("name").GetArrayLength());
        Assert.Equal(
            2,
            root.GetProperty("errors").GetProperty("email").GetArrayLength());
    }

    [Fact]
    public async Task ClientCancellationShouldNotBecomeInternalError()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.AddProblemDetails();

        using var serviceProvider = services.BuildServiceProvider();
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        var context = new DefaultHttpContext
        {
            RequestServices = serviceProvider,
            RequestAborted = cancellationSource.Token
        };

        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => Task.FromCanceled(cancellationSource.Token),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(
            context,
            serviceProvider.GetRequiredService<IProblemDetailsService>());

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
            "not-found" => AppException.NotFound("Jogo não encontrado."),
            "conflict" => AppException.Conflict(
                "O e-mail já está cadastrado."),
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
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.AddProblemDetails();

        using var serviceProvider = services.BuildServiceProvider();
        var context = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/tests/errors";
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => Task.FromException(exception),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(
            context,
            serviceProvider.GetRequiredService<IProblemDetailsService>());

        context.Response.Body.Position = 0;
        var document = await JsonDocument.ParseAsync(context.Response.Body);

        return new MiddlewareResult(context.Response.StatusCode, document);
    }

    private sealed record MiddlewareResult(
        int StatusCode,
        JsonDocument Document);
}
