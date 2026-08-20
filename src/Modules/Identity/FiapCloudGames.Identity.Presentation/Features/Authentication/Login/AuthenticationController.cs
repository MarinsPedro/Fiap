using FiapCloudGames.Identity.Application.Features.Authentication.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Identity.Presentation.Features.Authentication.Login;

/// <summary>
/// Controlador para a autenticação na plataforma.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : ControllerBase
{
    /// <summary>
    /// Executa a autenticação do usuário na plataforma Fiap Cloud Game.
    /// </summary>
    /// <param name="request">Objeto request para envio do e-mail e password para o procesos de autenticação.</param>
    /// <param name="service">Serviço do tipo LoginService.</param>
    /// <param name="cancellationToken">Token para cancelamento da requisição</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        [FromServices] LoginService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(request.ToInput(), cancellationToken);
        return Ok(result.ToResponse());
    }
}
