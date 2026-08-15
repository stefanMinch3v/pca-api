using pca.Domain.Common;

namespace pca.Domain.Contacts.ValueObjects;

/// <summary>
/// IBAN (International Bank Account Number) value object. Validates the
/// ISO 13616 (ISO 13616-1:2020) structure and mod-97 checksum, and is only ever created
/// through <see cref="Create"/>.
/// </summary>
public sealed record Iban
{
    // internal (not private) so the Application layer's FluentValidation
    // rules can reuse these instead of redeclaring the same bounds.
    internal const int MinLength = 15;
    internal const int MaxLength = 34;

    public const string RequiredErrorMessage = "IBAN is required.";
    public const string InvalidStructureMessage = "IBAN must start with a 2-letter country code and 2 check digits, followed by alphanumeric characters.";
    public const string InvalidChecksumMessage = "IBAN checksum is invalid.";

    public string Value { get; }
    public string CountryCode => Value[..2];

    private Iban(string value)
    {
        Value = value;
    }

    public static Iban Create(string iban)
    {
        var normalized = (iban ?? string.Empty).Replace(" ", string.Empty).ToUpperInvariant();

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            errors.Add(RequiredErrorMessage);
        }
        else
        {
            if (normalized.Length < MinLength || normalized.Length > MaxLength)
            {
                errors.Add($"IBAN must be between {MinLength} and {MaxLength} characters.");
            }

            if (!HasValidStructure(normalized))
            {
                errors.Add(InvalidStructureMessage);
            }

            if (errors.Count == 0 && !HasValidChecksum(normalized))
            {
                errors.Add(InvalidChecksumMessage);
            }
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }

        return new Iban(normalized);
    }

    private static bool HasValidStructure(string value) =>
        value.Length >= 4
        && char.IsLetter(value[0]) && char.IsLetter(value[1])
        && char.IsDigit(value[2]) && char.IsDigit(value[3])
        && value[4..].All(char.IsLetterOrDigit);

    /// <summary>
    /// ISO 13616 mod-97 checksum: move the first 4 characters to the end,
    /// convert letters to numbers (A=10 ... Z=35), then verify the resulting
    /// number mod 97 equals 1. The number is processed digit-by-digit to
    /// avoid overflowing standard integer types.
    /// </summary>
    private static bool HasValidChecksum(string value)
    {
        var rearranged = value[4..] + value[..4];

        var remainder = 0;
        foreach (var c in rearranged)
        {
            var digitValue = char.IsDigit(c) ? c - '0' : c - 'A' + 10;

            foreach (var digitChar in digitValue.ToString())
            {
                remainder = (remainder * 10 + (digitChar - '0')) % 97;
            }
        }

        return remainder == 1;
    }

    public override string ToString() => Value;
}
