using FiapCloudGames.Promotions.Application.Promotions;
using FiapCloudGames.Promotions.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Promotions.Presentation.Controllers;

[ApiController]
[Route("api/promotions")]
public sealed class PromotionsController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyList<PromotionSummary>>> ListActive([FromServices] ListActivePromotionsService service,CancellationToken cancellationToken) =>
        Ok(await service.ExecuteAsync(cancellationToken));

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<PromotionSummary>> Create(CreatePromotionRequest request,[FromServices] CreatePromotionService service,CancellationToken cancellationToken)
    {
        var promotion = await service.ExecuteAsync(
            new CreatePromotionInput(
                request.Name,
                request.DiscountPercent,
                request.StartsAtUtc,
                request.EndsAtUtc,
                request.GameIds),
            cancellationToken);
        return Created($"/api/promotions/{promotion.Id}", promotion);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("{id:guid}/end")]
    public async Task<IActionResult> End(Guid id,[FromServices] EndPromotionService service,CancellationToken cancellationToken)
    {
        await service.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}

public sealed record CreatePromotionRequest(string Name,decimal DiscountPercent,DateTimeOffset StartsAtUtc,DateTimeOffset EndsAtUtc,IReadOnlyCollection<Guid> GameIds);
