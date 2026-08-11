using FiapCloudGames.Library.Application.Features.UserLibrary.AcquireGame;
using FiapCloudGames.Library.Application.Features.UserLibrary.GetLibrary;
using FiapCloudGames.Presentation.Common.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Library.Presentation.Features.UserLibrary;

[ApiController]
[Authorize]
[Route("api/library")]
public sealed class LibraryController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserLibraryResponse>> Get(
        [FromServices] GetLibraryService service,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await service.ExecuteAsync(
            userId,
            cancellationToken);

        return Ok(result.ToResponse());
    }

    [HttpPost("games/{gameId:guid}")]
    public async Task<ActionResult<LibraryItemResponse>> Acquire(
        Guid gameId,
        [FromServices] AcquireGameService service,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await service.ExecuteAsync(
            userId,
            gameId,
            cancellationToken);
        var response = result.ToResponse();

        return CreatedAtAction(
            nameof(Get),
            response);
    }

}
