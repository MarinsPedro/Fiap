using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FiapCloudGames.Presentation.Common.Errors;
using Microsoft.IdentityModel.Tokens;

namespace FiapCloudGames.Api.IntegrationTests;

public sealed class ApiProblemDetailsTests :
    IClassFixture<FiapCloudGamesApiFactory>
{
    private readonly HttpClient _client;
    private readonly FiapCloudGamesApiFactory _factory;

    public ApiProblemDetailsTests(FiapCloudGamesApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task InvalidJsonShouldReturnOneValidationError()
    {
        using var content = new StringContent(
            "{ invalid-json",
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync(
            new Uri("/api/users", UriKind.Relative),
            content);

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.BadRequest,
            ApiProblemTypes.Validation,
            "Um ou mais dados são inválidos",
            "Verifique os dados informados.",
            expectedErrorCount: 1);
        var error = Assert.Single(problem.Errors!);
        Assert.Equal("O JSON enviado é inválido.", error.Message);
        Assert.Null(error.Field);
    }

    [Fact]
    public async Task DataAnnotationsShouldReturnMultipleFieldErrors()
    {
        var response = await _client.PostAsJsonAsync(
            new Uri("/api/users", UriKind.Relative),
            new
            {
                Name = "A",
                Email = "email-invalido",
                Password = "1234567"
            });

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.BadRequest,
            ApiProblemTypes.Validation,
            "Um ou mais dados são inválidos",
            "Verifique os dados informados.",
            expectedErrorCount: 3);

        Assert.Contains(problem.Errors!, error => error.Field == "name");
        Assert.Contains(problem.Errors!, error => error.Field == "email");
        Assert.Contains(problem.Errors!, error => error.Field == "password");
    }

    [Theory]
    [InlineData("abcdefgh")]
    [InlineData("12345678")]
    [InlineData("Abcdefg!")]
    [InlineData("Abcdefg1")]
    public async Task WeakPasswordShouldReturnPasswordValidationError(
        string password)
    {
        var response = await _client.PostAsJsonAsync(
            new Uri("/api/users", UriKind.Relative),
            new
            {
                Name = "Aluno FIAP",
                Email = "aluno@fiap.com.br",
                Password = password
            });

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.BadRequest,
            ApiProblemTypes.Validation,
            "Um ou mais dados são inválidos",
            "Verifique os dados informados.",
            expectedErrorCount: 1);
        var error = Assert.Single(problem.Errors!);
        Assert.Equal("password", error.Field);
        Assert.Equal(
            "A senha deve ter pelo menos 8 caracteres e conter letras, números e caracteres especiais.",
            error.Message);
    }

    [Fact]
    public async Task MissingTokenShouldReturnUnauthorizedProblem()
    {
        var response = await _client.GetAsync(
            new Uri("/api/users/me", UriKind.Relative));

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.Unauthorized,
            ApiProblemTypes.Unauthorized,
            "Não autenticado",
            "Token ausente, inválido ou expirado.");
        Assert.Null(problem.Errors);
    }

    [Fact]
    public async Task InvalidUserIdentifierShouldBeRejectedByApplication()
    {
        using var request = CreateAuthorizedRequest(
            HttpMethod.Get,
            "/api/users/me",
            "User",
            userId: "identificador-invalido");

        var response = await _client.SendAsync(request);

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.Unauthorized,
            ApiProblemTypes.Unauthorized,
            "Não autenticado",
            "O identificador do usuário autenticado é inválido.");
        Assert.Null(problem.Errors);
    }

    [Fact]
    public async Task InsufficientRoleShouldReturnForbiddenProblem()
    {
        var path = $"/api/users/{Guid.NewGuid()}";
        using var request = CreateAuthorizedRequest(
            HttpMethod.Get,
            path,
            "User");

        var response = await _client.SendAsync(request);

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.Forbidden,
            ApiProblemTypes.Forbidden,
            "Acesso negado",
            "O usuário autenticado não possui a permissão necessária.");
        Assert.Null(problem.Errors);
    }

    [Fact]
    public async Task UnknownRouteShouldReturnNotFoundProblem()
    {
        var response = await _client.GetAsync(
            new Uri("/rota-inexistente", UriKind.Relative));

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.NotFound,
            ApiProblemTypes.NotFound,
            "Recurso não encontrado",
            "O recurso informado não foi encontrado.");
        Assert.Null(problem.Errors);
    }

    [Fact]
    public async Task ServiceNotFoundShouldUseTheSameProblemContract()
    {
        var path = $"/api/games/{Guid.NewGuid()}";
        var response = await _client.GetAsync(
            new Uri(path, UriKind.Relative));

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.NotFound,
            ApiProblemTypes.NotFound,
            "Recurso não encontrado",
            "Jogo não encontrado.");
        Assert.Null(problem.Errors);
    }

    [Fact]
    public async Task InactiveGameShouldReturnConflictProblem()
    {
        var path = $"/api/games/{_factory.InactiveGameId}";
        using var request = CreateAuthorizedRequest(
            HttpMethod.Delete,
            path,
            "Administrator");

        var response = await _client.SendAsync(request);

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.Conflict,
            ApiProblemTypes.Conflict,
            "Conflito",
            "Jogo já está desativado.");
        Assert.Null(problem.Errors);
    }

    [Fact]
    public async Task InvalidPromotionPeriodShouldReturnBusinessRuleProblem()
    {
        var instant = DateTimeOffset.UtcNow.AddDays(1);
        using var request = CreateAuthorizedRequest(
            HttpMethod.Post,
            "/api/promotions",
            "Administrator");
        request.Content = JsonContent.Create(new
        {
            Name = "Promoção inválida",
            DiscountPercent = 10m,
            StartsAtUtc = instant,
            EndsAtUtc = instant,
            GameIds = new[] { Guid.NewGuid() }
        });

        var response = await _client.SendAsync(request);

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.UnprocessableEntity,
            ApiProblemTypes.BusinessRule,
            "Regra de negócio não atendida",
            "O fim da promoção deve ser posterior ao início.");
        Assert.Null(problem.Errors);
    }

    [Fact]
    public async Task UnexpectedExceptionShouldReturnSafeInternalProblem()
    {
        var response = await _client.GetAsync(
            new Uri("/api/games", UriKind.Relative));

        var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.InternalServerError,
            ApiProblemTypes.InternalServerError,
            "Erro interno",
            "Não foi possível concluir a operação.");
        Assert.Null(problem.Errors);
        Assert.DoesNotContain(
            "banco de dados",
            System.Text.Json.JsonSerializer.Serialize(problem),
            StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<ApiProblemDetails> AssertProblemAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        string expectedType,
        string expectedTitle,
        string expectedDetail,
        int? expectedErrorCount = null)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(
            ApiProblemDetailsContentTypes.Json,
            response.Content.Headers.ContentType?.MediaType);

        var payload = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ApiProblemDetails>(
            payload,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var typedProblem = Assert.IsType<ApiProblemDetails>(problem);
        using var document = JsonDocument.Parse(payload);

        Assert.Equal(expectedType, typedProblem.Type);
        Assert.Equal(expectedTitle, typedProblem.Title);
        Assert.Equal((int)expectedStatus, typedProblem.Status);
        Assert.Equal(expectedDetail, typedProblem.Detail);
        Assert.False(string.IsNullOrWhiteSpace(typedProblem.TraceId));
        Assert.False(document.RootElement.TryGetProperty("instance", out _));
        var hasErrors = document.RootElement.TryGetProperty(
            "errors",
            out var serializedErrors);

        if (expectedErrorCount is null)
        {
            Assert.False(hasErrors);
            Assert.Null(typedProblem.Errors);
        }
        else
        {
            Assert.True(hasErrors);
            Assert.Equal(
                expectedErrorCount.Value,
                serializedErrors.GetArrayLength());
            Assert.Equal(expectedErrorCount.Value, typedProblem.Errors?.Count);

            var typedErrors = typedProblem.Errors!.ToArray();
            var jsonErrors = serializedErrors.EnumerateArray().ToArray();
            for (var index = 0; index < typedErrors.Length; index++)
            {
                Assert.False(string.IsNullOrWhiteSpace(
                    typedErrors[index].Message));
                Assert.False(jsonErrors[index].TryGetProperty("code", out _));

                if (typedErrors[index].Field is null)
                {
                    Assert.False(jsonErrors[index].TryGetProperty(
                        "field",
                        out _));
                }
            }
        }

        return typedProblem;
    }

    private static HttpRequestMessage CreateAuthorizedRequest(
        HttpMethod method,
        string path,
        string role,
        string? userId = null)
    {
        var request = new HttpRequestMessage(
            method,
            new Uri(path, UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CreateToken(role, userId));
        return request;
    }

    private static string CreateToken(
        string role,
        string? userId)
    {
        var now = DateTime.UtcNow;
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(FiapCloudGamesApiFactory.JwtKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            FiapCloudGamesApiFactory.JwtIssuer,
            FiapCloudGamesApiFactory.JwtAudience,
            [
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId ?? Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
            ],
            now.AddMinutes(-1),
            now.AddMinutes(5),
            credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
