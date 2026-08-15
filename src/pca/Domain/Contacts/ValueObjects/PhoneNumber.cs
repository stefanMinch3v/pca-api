using System.Text.RegularExpressions;
using pca.Domain.Common;

namespace pca.Domain.Contacts.ValueObjects;

/// <summary>
/// Phone number value object. Accepts an optional leading '+' followed by
/// 7-15 digits (loosely aligned with E.164), and is only ever created
/// through <see cref="Create"/>.
/// </summary>
public sealed partial record PhoneNumber
{
    public const string InvalidFormatMessage =
        "Phone number must be a valid international number, e.g. +1234567890 (7-15 digits, optional leading '+').";

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        var normalized = Normalize(phoneNumber);

        if (string.IsNullOrWhiteSpace(normalized) || !PhoneNumberRegex().IsMatch(normalized))
        {
            throw new DomainValidationException(InvalidFormatMessage);
        }

        return new PhoneNumber(normalized);
    }

    private static string Normalize(string value) =>
        (value ?? string.Empty)
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .Replace("(", string.Empty)
            .Replace(")", string.Empty)
            .Trim();

    [GeneratedRegex(@"^\+?[1-9]\d{6,14}$")]
    private static partial Regex PhoneNumberRegex();

    public override string ToString() => Value;
}
