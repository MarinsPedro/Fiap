using FiapCloudGames.Catalog.Domain.ValueObjects;
using FiapCloudGames.Domain.Common;

namespace FiapCloudGames.Catalog.UnitTests.Domain;

public sealed class GamePriceTests
{
    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => GamePrice.Create(-0.01m));

        Assert.Equal("O preço base não pode ser negativo.", exception.Message);
    }

    [Fact]
    public void Create_WithZero_ShouldCreatePrice()
    {
        var price = GamePrice.Create(0m);

        Assert.Equal(0m, price.Amount);
    }

    [Fact]
    public void Create_WithMidpointAmount_ShouldRoundToEven()
    {
        var priceRoundedUp = GamePrice.Create(99.995m);
        var priceRoundedDown = GamePrice.Create(99.985m);

        Assert.Equal(100.00m, priceRoundedUp.Amount);
        Assert.Equal(99.98m, priceRoundedDown.Amount);
    }
}
