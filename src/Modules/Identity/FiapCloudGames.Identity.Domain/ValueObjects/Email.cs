using System.Net.Mail;

namespace FiapCloudGames.Identity.Domain.ValueObjects;

public readonly record struct Email
{
    private Email(string value) => Value = value;

    public string Value { get; }

    public static Email Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var normalized = value.Trim().ToLowerInvariant();

        if (!MailAddress.TryCreate(normalized, out var address) ||
            !string.Equals(address.Address, normalized, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("O e-mail informado é inválido.", nameof(value));
        }

        return new Email(normalized);
    }

    public override string ToString() => Value;
}
