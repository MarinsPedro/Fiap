using FiapCloudGames.Identity.Application.Features.Authentication.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Identity.Presentation.Features.Authentication.Login;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        [FromServices] LoginService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            request.ToInput(),
            cancellationToken);

        return Ok(result.ToResponse());
    }
}
