using System.Security.Claims;
using FiapCloudGames.Identity.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Identity.Presentation.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<CreateUserOutput>> Create(CreateUserRequest request,[FromServices] CreateUserService service,CancellationToken cancellationToken)
    {
        var output = await service.ExecuteAsync(new CreateUserInput(request.Name, request.Email, request.Password),cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = output.Id }, output);
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id,[FromServices] GetUserService service,CancellationToken cancellationToken)
    {
        var user = await service.ExecuteAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult> GetCurrent([FromServices] GetUserService service,CancellationToken cancellationToken)
    {
        var id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await service.ExecuteAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id,[FromServices] DeactivateUserService service,CancellationToken cancellationToken)
    {
        await service.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}

public sealed record CreateUserRequest(string Name, string Email, string Password);
