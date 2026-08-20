using FiapCloudGames.Identity.Application.Features.Users.CreateUser;
using FiapCloudGames.Identity.Application.Features.Users.DeactivateUser;
using FiapCloudGames.Identity.Application.Features.Users.GetUser;
using FiapCloudGames.Identity.Application.Features.Users.UpdateUser;
using FiapCloudGames.Identity.Presentation.Features.Authentication.Login;
using FiapCloudGames.Identity.Presentation.Features.Users.CreateUser;
using FiapCloudGames.Identity.Presentation.Features.Users.UpdateUser;
using FiapCloudGames.Presentation.Common.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Identity.Presentation.Features.Users;

/// <summary>
/// Controlador para manipulação dos dados de usuário.
/// </summary>
[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    /// <summary>
    /// Cria um usuário no sistema com o papel usuário.
    /// </summary>
    /// <param name="request">Objeto request para envio com os dados de criação de usuário.</param>
    /// <param name="service">Serviço do tipo CreateUserService.</param>
    /// <param name="cancellationToken">Token para cancelamento.</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] CreateUserRequest request,
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

    /// <summary>
    /// Atualiza os dados de usuário, somente para o usuário que está logado.
    /// </summary>
    /// <param name="request">Objeto request para envio com os dados para atualização de usuário.</param>
    /// <param name="service">Serviço do tipo UpdateUserService.</param>
    /// <param name="cancellationToken">Token para cancelamento.</param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Obtém os detalhes de um usuário que só podem ser consultados por um usuário adminstrador.
    /// </summary>
    /// <param name="id">O id do tipo Guid que identitica um usuário.</param>
    /// <param name="service">Serviço do tipo GetUserService.</param>
    /// <param name="cancellationToken">Token para cancelamento.</param>
    /// <returns></returns>
    [Authorize(Roles = "Administrator")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> GetById(
        [FromRoute] Guid id,
        [FromServices] GetUserService service,
        CancellationToken cancellationToken)
    {
        var user = await service.ExecuteAsync(id, cancellationToken);

        return user is null
            ? NotFound()  //TODO: essa resposta deve ser movida para a camada de service, e aplicar um AppException.NotFound.
            : Ok(user.ToResponse());
    }

    /// <summary>
    /// Obtém os dados do usuário que está logado na plataforma.
    /// </summary>
    /// <param name="service">Serviço do tipo GetUserService.</param>
    /// <param name="cancellationToken">Token para cancelamento.</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
            ? NotFound() //TODO: essa resposta deve ser movida para a camada de service, e aplicar um AppException.NotFound.
            : Ok(user.ToResponse());
    }

    /// <summary>
    /// Desativação (exclusão lógica) de usuário que somente é realizado por um usuário administrador.
    /// </summary>
    /// <param name="id">O id do tipo Guid que identitica um usuário.</param>
    /// <param name="service">Serviço do tipo DeactivateUserService.</param>
    /// <param name="cancellationToken">Token para cancelamento.</param>
    /// <returns></returns>
    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deactivate(
        [FromRoute] Guid id,
        [FromServices] DeactivateUserService service,
        CancellationToken cancellationToken)
    {
        await service.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
