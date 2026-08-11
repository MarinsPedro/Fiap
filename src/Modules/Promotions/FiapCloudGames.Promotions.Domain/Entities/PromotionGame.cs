namespace FiapCloudGames.Promotions.Domain.Entities;

public sealed class PromotionGame
{
    private PromotionGame()
    {
    }

    internal PromotionGame(
        Guid promotionId,
        Guid gameId)
    {
        PromotionId = promotionId;
        GameId = gameId;
    }

    public Guid PromotionId { get; private set; }
    public Guid GameId { get; private set; }
}
