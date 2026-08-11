using FiapCloudGames.Promotions.Application.Features.Promotions.CreatePromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.EndPromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.GetPromotion;
using FiapCloudGames.Promotions.Application.Features.Promotions.ListActivePromotions;
using FiapCloudGames.Promotions.Presentation.Features.Promotions.CreatePromotion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Promotions.Presentation.Features.Promotions;

[ApiController]
[Route("api/promotions")]
public sealed class PromotionsController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyList<PromotionResponse>>>
        ListActive(
            [FromServices] ListActivePromotionsService service,
            CancellationToken cancellationToken)
    {
        var results = await service.ExecuteAsync(cancellationToken);

        return Ok(results.Select(PromotionResponseMappings.ToResponse));
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PromotionResponse>> GetById(
        Guid id,
        [FromServices] GetPromotionService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(id, cancellationToken);

        return result is null
            ? NotFound()
            : Ok(result.ToResponse());
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<PromotionResponse>> Create(
        CreatePromotionRequest request,
        [FromServices] CreatePromotionService service,
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

    [Authorize(Roles = "Administrator")]
    [HttpPost("{id:guid}/end")]
    public async Task<IActionResult> End(
        Guid id,
        [FromServices] EndPromotionService service,
        CancellationToken cancellationToken)
    {
        await service.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
