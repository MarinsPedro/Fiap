using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Api.IntegrationTests.Support;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("_tests/errors")]
public sealed class TestErrorsController : ControllerBase
{
    [HttpGet("unhandled")]
    public IActionResult Unhandled() =>
        throw new InvalidOperationException(
            "Detalhe técnico que não pode aparecer na resposta.");
}
