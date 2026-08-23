using FiapCloudGames.Domain.Common;
using FiapCloudGames.Library.Domain.ValueObjects;

namespace FiapCloudGames.Library.UnitTests.Domain;

public sealed class AcquisitionPriceTests
{
    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowBusinessRule()
    {
        var exception = Assert.Throws<DomainRuleViolationException>(
            () => AcquisitionPrice.Create(-0.01m));

        Assert.Equal("O preço pago não pode ser negativo.", exception.Message);
    }

    [Fact]
    public void Create_WithZero_ShouldCreatePrice()
    {
        var price = AcquisitionPrice.Create(0m);

        Assert.Equal(0m, price.Amount);
    }

    [Fact]
    public void Create_WithMidpointAmount_ShouldRoundToEven()
    {
        var roundedUp = AcquisitionPrice.Create(79.995m);
        var roundedDown = AcquisitionPrice.Create(79.985m);

        Assert.Equal(80.00m, roundedUp.Amount);
        Assert.Equal(79.98m, roundedDown.Amount);
    }
}
