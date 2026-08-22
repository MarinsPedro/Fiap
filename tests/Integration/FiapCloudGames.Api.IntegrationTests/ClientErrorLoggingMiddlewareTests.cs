using System.Diagnostics;
using FiapCloudGames.Api.Middlewares;
using FiapCloudGames.Presentation.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Api.IntegrationTests;

public sealed class ClientErrorLoggingMiddlewareTests
{
    [Theory]
    [InlineData(StatusCodes.Status400BadRequest, LogLevel.Information)]
    [InlineData(StatusCodes.Status401Unauthorized, LogLevel.Information)]
    [InlineData(StatusCodes.Status403Forbidden, LogLevel.Warning)]
    [InlineData(StatusCodes.Status404NotFound, LogLevel.Information)]
    [InlineData(StatusCodes.Status409Conflict, LogLevel.Information)]
    [InlineData(StatusCodes.Status422UnprocessableEntity, LogLevel.Information)]
    [InlineData(StatusCodes.Status429TooManyRequests, LogLevel.Warning)]
    public async Task ClientErrorsShouldUseTheExpectedLogLevel(
        int status,
        LogLevel expectedLevel)
    {
        var logger = new TestLogger<ClientErrorLoggingMiddleware>();
        var middleware = new ClientErrorLoggingMiddleware(
            context =>
            {
                context.Response.StatusCode = status;
                ApiProblemDetailsFactory.CreateStatusCode(context, status);
                return Task.CompletedTask;
            },
            logger);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(expectedLevel, entry.Level);
        Assert.Contains($"({status})", entry.Message, StringComparison.Ordinal);
        Assert.Contains("POST /api/tests", entry.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("TraceId:", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ProblemDetailsShouldUseTheW3CTraceIdOnly()
    {
        using var activity = new Activity("problem-details-test")
            .SetIdFormat(ActivityIdFormat.W3C)
            .Start();
        var context = CreateContext();

        var problemDetails = ApiProblemDetailsFactory.CreateStatusCode(
            context,
            StatusCodes.Status400BadRequest);

        Assert.Equal(activity.TraceId.ToString(), problemDetails.TraceId);
        Assert.NotEqual(activity.Id, problemDetails.TraceId);
        Assert.Equal(32, problemDetails.TraceId.Length);
    }

    [Fact]
    public async Task ValidationLogShouldExposeFieldsButNotMessages()
    {
        const string sensitiveMessage =
            "Valor inválido que não deve aparecer no log.";
        var logger = new TestLogger<ClientErrorLoggingMiddleware>();
        var middleware = new ClientErrorLoggingMiddleware(
            context =>
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                ApiProblemDetailsFactory.CreateValidation(
                    context,
                    [
                        new ApiError
                        {
                            Message = sensitiveMessage,
                            Field = "name"
                        },
                        new ApiError
                        {
                            Message = sensitiveMessage,
                            Field = "email"
                        }
                    ]);
                return Task.CompletedTask;
            },
            logger);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains("name,email", entry.Message, StringComparison.Ordinal);
        Assert.Contains("Erros: 2", entry.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(
            sensitiveMessage,
            entry.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task ServerErrorsShouldNotBeLoggedAsClientErrors()
    {
        var logger = new TestLogger<ClientErrorLoggingMiddleware>();
        var middleware = new ClientErrorLoggingMiddleware(
            context =>
            {
                context.Response.StatusCode =
                    StatusCodes.Status500InternalServerError;
                ApiProblemDetailsFactory.CreateStatusCode(
                    context,
                    StatusCodes.Status500InternalServerError);
                return Task.CompletedTask;
            },
            logger);

        await middleware.InvokeAsync(CreateContext());

        Assert.Empty(logger.Entries);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/tests";
        return context;
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<LogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull =>
            NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            Entries.Add(new LogEntry(logLevel, formatter(state, exception)));
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        public void Dispose()
        {
        }
    }

    private sealed record LogEntry(
        LogLevel Level,
        string Message);
}
