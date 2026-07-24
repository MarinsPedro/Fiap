using FiapCloudGames.Catalog.Application.Games;
using FiapCloudGames.Catalog.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Catalog.Presentation.Controllers;

[ApiController]
[Route("api/games")]
public sealed class GamesController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GameSummary>>> List([FromServices] ListGamesService service,CancellationToken cancellationToken) =>
        Ok(await service.ExecuteAsync(onlyActive: true, cancellationToken));

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GameSummary>> Get(Guid id,[FromServices] GetGameService service,CancellationToken cancellationToken)
    {
        var game = await service.ExecuteAsync(id, cancellationToken);
        return game is null ? NotFound() : Ok(game);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<GameSummary>> Create(GameRequest request,[FromServices] CreateGameService service,CancellationToken cancellationToken)
    {
        var game = await service.ExecuteAsync(new CreateGameInput(request.Title, request.Description, request.Category, request.BasePrice),cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = game.Id }, game);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<GameSummary>> Update(Guid id,UpdateGameRequest request,[FromServices] UpdateGameService service,CancellationToken cancellationToken) =>
        Ok(await service.ExecuteAsync(id,new UpdateGameInput(request.Title, request.Description, request.Category, request.BasePrice, request.IsActive),cancellationToken));
}

public sealed record GameRequest(string Title, string Description, string Category, decimal BasePrice);
public sealed record UpdateGameRequest(string Title, string Description, string Category, decimal BasePrice, bool IsActive);
