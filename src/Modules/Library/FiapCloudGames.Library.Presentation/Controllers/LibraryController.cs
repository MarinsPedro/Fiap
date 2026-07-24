using System.Security.Claims;
using FiapCloudGames.Library.Application.Games;
using FiapCloudGames.Library.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Library.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/library")]
public sealed class LibraryController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserLibrarySummary>> Get([FromServices] GetLibraryService service,CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await service.ExecuteAsync(userId, cancellationToken));
    }

    [HttpPost("games/{gameId:guid}")]
    public async Task<ActionResult<LibraryItemSummary>> Acquire(Guid gameId,[FromServices] AcquireGameService service,CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var item = await service.ExecuteAsync(userId, gameId, cancellationToken);
        return Created($"/api/library/games/{item.GameId}", item);
    }
}
