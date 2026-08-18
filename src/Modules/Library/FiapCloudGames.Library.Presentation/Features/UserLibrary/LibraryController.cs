using FiapCloudGames.Library.Application.Features.UserLibrary.AcquireGame;
using FiapCloudGames.Library.Application.Features.UserLibrary.GetLibrary;
using FiapCloudGames.Presentation.Common.Authentication;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<UserLibraryResponse>> Get(
        [FromServices] GetLibraryService service,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await service.ExecuteAsync(userId, cancellationToken);
        return Ok(result.ToResponse());
    }

    /// <summary>
    /// Obtém os detalhes do jogo da biblioteca do usuário logado.
    /// </summary>
    /// <param name="gameId">O ID do jogo.</param>
    /// <param name="service">Serviço do tipo AcquireGameService.</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Retorna os detalhes do jogo da biblioteca do usuário logado.</returns>
    [HttpPost("games/{gameId:guid}")]
    public async Task<ActionResult<LibraryItemResponse>> Acquire(
        [FromRoute] Guid gameId,
        [FromServices] AcquireGameService service,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await service.ExecuteAsync(userId, gameId, cancellationToken);
        var response = result.ToResponse();

        return CreatedAtAction(nameof(Get), response);
    }
}
