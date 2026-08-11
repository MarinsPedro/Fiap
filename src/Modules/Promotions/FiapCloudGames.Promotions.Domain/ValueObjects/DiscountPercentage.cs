using FiapCloudGames.Domain.Common;

namespace FiapCloudGames.Promotions.Domain.ValueObjects;

public sealed record DiscountPercentage
{
    private DiscountPercentage(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public static DiscountPercentage Create(decimal value)
    {
        if (value is <= 0 or > 100)
        {
            throw new DomainRuleViolationException(
                "O desconto deve ser maior que zero e menor ou igual a 100%.");
        }

        return new DiscountPercentage(
            decimal.Round(
                value,
                2,
                MidpointRounding.ToEven));
    }

    public decimal ApplyTo(decimal basePrice)
    {
        if (basePrice < 0)
        {
            throw new DomainRuleViolationException(
                "O preço base não pode ser negativo.");
        }

        return decimal.Round(
            basePrice * (1 - (Value / 100m)),
            2,
            MidpointRounding.ToEven);
    }

    public override string ToString() =>
        Value.ToString("0.##");
}
