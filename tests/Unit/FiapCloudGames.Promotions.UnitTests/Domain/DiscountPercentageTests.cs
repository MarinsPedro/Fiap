using System.Globalization;
using FiapCloudGames.Domain.Common;
using FiapCloudGames.Promotions.Domain.ValueObjects;

namespace FiapCloudGames.Promotions.UnitTests.Domain;

public sealed class DiscountPercentageTests
{
    [Theory]
    [InlineData("-0.01")]
    [InlineData("0")]
    [InlineData("100.01")]
    public void Create_OutsideAllowedRange_ShouldThrowBusinessRule(
        string value)
    {
        var amount = decimal.Parse(value, CultureInfo.InvariantCulture);

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => DiscountPercentage.Create(amount));

        Assert.Equal(
            "O desconto deve ser maior que zero e menor ou igual a 100%.",
            exception.Message);
    }

    [Theory]
    [InlineData("0.01")]
    [InlineData("100")]
    public void Create_AtAllowedBoundary_ShouldCreateDiscount(string value)
    {
        var amount = decimal.Parse(value, CultureInfo.InvariantCulture);

        var discount = DiscountPercentage.Create(amount);

        Assert.Equal(amount, discount.Value);
    }

    [Fact]
    public void Create_WithMidpointValue_ShouldRoundToEven()
    {
        var roundedUp = DiscountPercentage.Create(25.995m);
        var roundedDown = DiscountPercentage.Create(25.985m);

        Assert.Equal(26.00m, roundedUp.Value);
        Assert.Equal(25.98m, roundedDown.Value);
    }

    [Fact]
    public void ApplyTo_WithValidPrice_ShouldCalculateAndRoundDiscount()
    {
        var discount = DiscountPercentage.Create(25m);

        var finalPrice = discount.ApplyTo(99.99m);

        Assert.Equal(74.99m, finalPrice);
    }

    [Fact]
    public void ApplyTo_WithZeroPrice_ShouldReturnZero()
    {
        var discount = DiscountPercentage.Create(100m);

        var finalPrice = discount.ApplyTo(0m);

        Assert.Equal(0m, finalPrice);
    }

    [Fact]
    public void ApplyTo_WithNegativePrice_ShouldThrowBusinessRule()
    {
        var discount = DiscountPercentage.Create(25m);

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => discount.ApplyTo(-0.01m));

        Assert.Equal("O preço base não pode ser negativo.", exception.Message);
    }
}
