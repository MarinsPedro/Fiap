using FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.EndPromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.GetPromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.ListActivePromotions;
using FiapCloudGames.Promotions.Presentation.Features.Promotions.CreatePromotion;
using FiapCloudGames.Presentation.Common.Errors;
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
    [ProducesResponseType(typeof(IReadOnlyList<PromotionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError, ApiProblemDetailsContentTypes.Json)]
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
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status401Unauthorized, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status403Forbidden, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status404NotFound, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError, ApiProblemDetailsContentTypes.Json)]
    public async Task<ActionResult<PromotionResponse>> GetById(
        [FromRoute] Guid id,
        [FromServices] GetPromotionService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(id, cancellationToken);
        return Ok(result.ToResponse());
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
    [ProducesResponseType(typeof(PromotionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status400BadRequest, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status401Unauthorized, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status403Forbidden, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status404NotFound, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status422UnprocessableEntity, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError, ApiProblemDetailsContentTypes.Json)]
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
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status401Unauthorized, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status403Forbidden, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status404NotFound, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status422UnprocessableEntity, ApiProblemDetailsContentTypes.Json)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError, ApiProblemDetailsContentTypes.Json)]
    public async Task<IActionResult> End(
        [FromRoute] Guid id,
        [FromServices] EndPromotionService service,
        CancellationToken cancellationToken)
    {
        await service.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
