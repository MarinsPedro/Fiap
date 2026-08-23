using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FiapCloudGames.Api.IntegrationTests.Support;
using FiapCloudGames.Presentation.Common.Errors;

namespace FiapCloudGames.Api.IntegrationTests.Host;

public sealed class ApiProblemDetailsTests :
    IClassFixture<FiapCloudGamesApiFactory>
{
    private readonly HttpClient _client;

    public ApiProblemDetailsTests(FiapCloudGamesApiFactory factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task InvalidJson_ShouldReturnValidationProblem()
    {
        using var content = new StringContent(
            "{ invalid-json",
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync(
            new Uri("/api/users", UriKind.Relative),
            content);

        var problem = await ApiProblemAssertions.AssertAsync(
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
    public async Task InvalidRequest_ShouldPreserveFieldValidationContract()
    {
        var response = await _client.PostAsJsonAsync(
            new Uri("/api/users", UriKind.Relative),
            new
            {
                Name = "A",
                Email = "email-invalido",
                Password = "1234567"
            });

        var problem = await ApiProblemAssertions.AssertAsync(
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

    [Fact]
    public async Task UnknownRoute_ShouldReturnNotFoundProblem()
    {
        var response = await _client.GetAsync(
            new Uri("/rota-inexistente", UriKind.Relative));

        await ApiProblemAssertions.AssertAsync(
            response,
            HttpStatusCode.NotFound,
            ApiProblemTypes.NotFound,
            "Recurso não encontrado",
            "O recurso informado não foi encontrado.");
    }

    [Fact]
    public async Task UnhandledException_ShouldReturnSafeInternalProblem()
    {
        var response = await _client.GetAsync(
            new Uri("/_tests/errors/unhandled", UriKind.Relative));

        var problem = await ApiProblemAssertions.AssertAsync(
            response,
            HttpStatusCode.InternalServerError,
            ApiProblemTypes.InternalServerError,
            "Erro interno",
            "Não foi possível concluir a operação.");
        Assert.DoesNotContain(
            "detalhe técnico",
            JsonSerializer.Serialize(problem),
            StringComparison.OrdinalIgnoreCase);
    }
}
