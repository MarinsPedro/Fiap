using FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.EndPromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.GetPromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.ListActivePromotions;
using FiapCloudGames.Promotions.Presentation.Features.Promotions.CreatePromotion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Promotions.Presentation.Features.Promotions;

/// <summary>
/// Controlador reponsável por gerenciar as promoções para os jogos da plataforma.
/// </summary>
[ApiController]
[Route("api/promotions")]
public sealed class PromotionsController : ControllerBase
{
    /// <summary>
    /// Lista todas as promoções ativas.
    /// </summary>
    /// <param name="service">Serviço responsável por listar as promoções ativas.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Uma lista de respostas de promoção.</returns>
    [AllowAnonymous]
    [HttpGet("active")]
    [ProducesResponseType(typeof(PromotionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<PromotionResponse>>> ListActive(
        [FromServices] ListActivePromotionsService service,
        CancellationToken cancellationToken)
    {
        var results = await service.ExecuteAsync(cancellationToken);
        return Ok(results.Select(PromotionResponseMappings.ToResponse));
    }

    /// <summary>
    /// Obtém uma promoção pelo seu identificador único (ID), consultado somente pelo administrador.
    /// </summary>
    /// <param name="id">O identificador único da promoção.</param>
    /// <param name="service">Serviço responsável por obter a promoção.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Uma resposta de promoção.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PromotionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PromotionResponse>> GetById(
        [FromRoute] Guid id,
        [FromServices] GetPromotionService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(id, cancellationToken);
        return result is null
            ? NotFound() //TODO: essa resposta deve ser movida para a camada de service, e aplicar um AppException.NotFound.
            : Ok(result.ToResponse());
    }

    /// <summary>
    /// Cria uma nova promoção, operações realizadas somente pelo administrador.
    /// </summary>
    /// <param name="request">A solicitação de criação de promoção.</param>
    /// <param name="service">Serviço responsável por criar a promoção.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Uma resposta de promoção.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [ProducesResponseType(typeof(PromotionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PromotionResponse>> Create(
        [FromBody] CreatePromotionRequest request,
        [FromServices] CreatePromotionService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(request.ToInput(), cancellationToken);
        var response = result.ToResponse();

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }

    /// <summary>
    /// Encerra uma promoção existente, operações realizadas somente pelo administrador.
    /// </summary>
    /// <param name="id">O identificador único da promoção.</param>
    /// <param name="service">Serviço responsável por encerrar a promoção.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Uma ação resultante.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpPost("{id:guid}/end")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> End(
        [FromRoute] Guid id,
        [FromServices] EndPromotionService service,
        CancellationToken cancellationToken)
    {
        await service.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
