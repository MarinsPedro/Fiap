using FiapCloudGames.Domain.Common;

namespace FiapCloudGames.Library.Domain.ValueObjects;

public readonly record struct AcquisitionPrice
{
    private AcquisitionPrice(decimal amount)
    {
        Amount = amount;
    }

    public decimal Amount { get; }

    public static AcquisitionPrice Create(decimal amount)
    {
        if (amount < 0)
        {
            throw new DomainRuleViolationException(
                "O preço pago não pode ser negativo.");
        }

        return new AcquisitionPrice(
            decimal.Round(
                amount,
                2,
                MidpointRounding.ToEven));
    }

    public override string ToString() =>
        Amount.ToString("0.00");
}
