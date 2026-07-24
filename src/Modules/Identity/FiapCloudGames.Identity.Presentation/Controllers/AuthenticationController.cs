using FiapCloudGames.Identity.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Identity.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginOutput>> Login(LoginRequest request,[FromServices] LoginService service,CancellationToken cancellationToken) =>
        Ok(await service.ExecuteAsync(new LoginInput(request.Email, request.Password), cancellationToken));
}

public sealed record LoginRequest(string Email, string Password);
