using FiapCloudGames.Catalog.Application.Features.Games.CreateGame;
using FiapCloudGames.Catalog.Application.Features.Games.DeactivateGame;
using FiapCloudGames.Catalog.Application.Features.Games.GetGame;
using FiapCloudGames.Catalog.Application.Features.Games.ListGames;
using FiapCloudGames.Catalog.Application.Features.Games.UpdateGame;
using FiapCloudGames.Catalog.Presentation.Features.Games.CreateGame;
using FiapCloudGames.Catalog.Presentation.Features.Games.UpdateGame;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Catalog.Presentation.Features.Games;

/// <summary>
/// Controlador para manipulação dos dados de jogos.
/// </summary>
[ApiController]
[Route("api/games")]
public sealed class GamesController : ControllerBase
{
    /// <summary>
    /// Lista todos os jogos ativos.
    /// </summary>
    /// <param name="service">Serviço para listar os jogos.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de jogos ativos.</returns>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<GameResponse>>> List(
        [FromServices] ListGamesService service,
        CancellationToken cancellationToken)
    {
        var results = await service.ExecuteAsync(onlyActive: true, cancellationToken);
        return Ok(results.Select(GameResponseMappings.ToResponse));
    }

    /// <summary>
    /// Obtém os detalhes de um jogo específico pelo seu ID.
    /// </summary>
    /// <param name="id">ID do jogo.</param>
    /// <param name="service">Serviço para obter os detalhes do jogo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Detalhes do jogo.</returns>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GameResponse>> Get(
        [FromRoute] Guid id,
        [FromServices] GetGameService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(id, cancellationToken);

        return result is null
            ? NotFound()  //TODO: essa resposta deve ser movida para a camada de service, e aplicar um AppException.NotFound.
            : Ok(result.ToResponse());
    }

    /// <summary>
    /// Cria um novo jogo.
    /// </summary>
    /// <param name="request">Requisição para criar um novo jogo.</param>
    /// <param name="service">Serviço para criar um novo jogo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Detalhes do jogo criado.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GameResponse>> Create(
        [FromBody] CreateGameRequest request,
        [FromServices] CreateGameService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            request.ToInput(),
            cancellationToken);
        var response = result.ToResponse();

        return CreatedAtAction(
            nameof(Get),
            new { id = response.Id },
            response);
    }

    /// <summary>
    /// Atualiza os detalhes de um jogo existente.
    /// </summary>
    /// <param name="id">ID do jogo.</param>
    /// <param name="request">Requisição para atualizar o jogo.</param>
    /// <param name="service">Serviço para atualizar o jogo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Detalhes do jogo atualizado.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GameResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateGameRequest request,
        [FromServices] UpdateGameService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            id,
            request.ToInput(),
            cancellationToken);

        return Ok(result.ToResponse());
    }


    /// <summary>
    /// Desativação (exclusão lógica) de jogos que somente é realizada por um usuário administrador.
    /// </summary>
    /// <param name="id">ID do jogo a ser desativado.</param>
    /// <param name="service">Serviço para desativar o jogo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deactivate(
        [FromRoute] Guid id,
        [FromServices] DeactivateGameService service,
        CancellationToken cancellationToken)
    {
        await service.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
