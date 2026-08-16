using FiapCloudGames.Identity.Application.Features.Users.CreateUser;
using FiapCloudGames.Identity.Application.Features.Users.DeactivateUser;
using FiapCloudGames.Identity.Application.Features.Users.GetUser;
using FiapCloudGames.Identity.Application.Features.Users.UpdateUser;
using FiapCloudGames.Identity.Presentation.Features.Users.CreateUser;
using FiapCloudGames.Identity.Presentation.Features.Users.UpdateUser;
using FiapCloudGames.Presentation.Common.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Identity.Presentation.Features.Users;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest request,
        [FromServices] CreateUserService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            request.ToInput(),
            cancellationToken);
        var response = result.ToResponse();

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> Update(
        [FromBody] UpdateUserRequest request,
        [FromServices] UpdateUserService service,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var id))
        {
            return Unauthorized();
        }

        var result = await service.ExecuteAsync(
            id,
            request.ToInput(),
            cancellationToken);

        return Ok(result.ToResponse());
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(
        Guid id,
        [FromServices] GetUserService service,
        CancellationToken cancellationToken)
    {
        var user = await service.ExecuteAsync(id, cancellationToken);

        return user is null
            ? NotFound()
            : Ok(user.ToResponse());
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrent(
        [FromServices] GetUserService service,
        CancellationToken cancellationToken)
    {
        if (!User.TryGetUserId(out var id))
        {
            return Unauthorized();
        }

        var user = await service.ExecuteAsync(id, cancellationToken);

        return user is null
            ? NotFound()
            : Ok(user.ToResponse());
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        [FromServices] DeactivateUserService service,
        CancellationToken cancellationToken)
    {
        await service.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
