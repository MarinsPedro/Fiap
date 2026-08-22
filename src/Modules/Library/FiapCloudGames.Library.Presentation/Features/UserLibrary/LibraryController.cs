using FiapCloudGames.Library.Application.Features.UserLibrary.AcquireGame;
using FiapCloudGames.Library.Application.Features.UserLibrary.GetCurrentLibrary;
using FiapCloudGames.Presentation.Common.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Library.Presentation.Features.UserLibrary;

/// <summary>
/// Controlador para manipulação dos dados de biblioteca de jogos de usuário.
/// </summary>
[ApiController]
[Authorize]
[Route("api/library")]
public sealed class LibraryController : ControllerBase
{
    /// <summary>
    /// Obtém a biblioteca de jogos do usuário logado.
    /// </summary>
    /// <param name="service">Serviço do tipo GetLibraryService</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Retorna a listagem da biblioteca de jogos do usuário.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserLibraryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status401Unauthorized, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError, ApiProblemDetailsContentTypes.Json)]
    public async Task<ActionResult<UserLibraryResponse>> Get(
        [FromServices] GetCurrentLibraryService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(cancellationToken);
        return Ok(result.ToResponse());
    }

    /// <summary>
    /// Adiciona um jogo para a biblioteca do usuário logado.
    /// </summary>
    /// <param name="gameId">O ID do jogo.</param>
    /// <param name="service">Serviço do tipo AcquireGameService.</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Retorna os detalhes do jogo adicinado na biblioteca do usuário logado.</returns>
    [Authorize]
    [HttpPost("games/{gameId:guid}")]
    [ProducesResponseType(typeof(LibraryItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status401Unauthorized, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status404NotFound, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status409Conflict, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status422UnprocessableEntity, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError, ApiProblemDetailsContentTypes.Json)]
    public async Task<ActionResult<LibraryItemResponse>> Acquire(
        [FromRoute] Guid gameId,
        [FromServices] AcquireGameService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(gameId, cancellationToken);
        var response = result.ToResponse();
        return CreatedAtAction(nameof(Get), response);
    }
}
