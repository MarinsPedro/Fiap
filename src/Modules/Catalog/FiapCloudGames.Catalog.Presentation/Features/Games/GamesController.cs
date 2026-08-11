using FiapCloudGames.Catalog.Application.Features.Games.CreateGame;
using FiapCloudGames.Catalog.Application.Features.Games.GetGame;
using FiapCloudGames.Catalog.Application.Features.Games.ListGames;
using FiapCloudGames.Catalog.Application.Features.Games.UpdateGame;
using FiapCloudGames.Catalog.Presentation.Features.Games.CreateGame;
using FiapCloudGames.Catalog.Presentation.Features.Games.UpdateGame;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Catalog.Presentation.Features.Games;

[ApiController]
[Route("api/games")]
public sealed class GamesController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GameResponse>>> List(
        [FromServices] ListGamesService service,
        CancellationToken cancellationToken)
    {
        var results = await service.ExecuteAsync(
            onlyActive: true,
            cancellationToken);

        return Ok(results.Select(GameResponseMappings.ToResponse));
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GameResponse>> Get(
        Guid id,
        [FromServices] GetGameService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(id, cancellationToken);

        return result is null
            ? NotFound()
            : Ok(result.ToResponse());
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<GameResponse>> Create(
        CreateGameRequest request,
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

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<GameResponse>> Update(
        Guid id,
        UpdateGameRequest request,
        [FromServices] UpdateGameService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            id,
            request.ToInput(),
            cancellationToken);

        return Ok(result.ToResponse());
    }
}
