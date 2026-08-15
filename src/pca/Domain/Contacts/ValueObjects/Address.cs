using pca.Domain.Common;

namespace pca.Domain.Contacts.ValueObjects;

/// <summary>
/// Postal address value object. Immutable and only ever created through
/// <see cref="Create"/>, which validates and normalizes every part.
/// </summary>
public sealed record Address
{
    // internal (not private) so the Application layer's FluentValidation
    // rules can reuse these instead of redeclaring the same bounds.
    internal const int MaxStreetLength = 200;
    internal const int MaxCityLength = 100;
    internal const int MaxPostalCodeLength = 20;
    internal const int MaxCountryLength = 100;

    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; } // Can be as a static Dic with all contries or Db table

    private Address(string street, string city, string postalCode, string country)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    public static Address Create(string street, string city, string postalCode, string country)
    {
        street = (street ?? string.Empty).Trim();
        city = (city ?? string.Empty).Trim();
        postalCode = (postalCode ?? string.Empty).Trim();
        country = (country ?? string.Empty).Trim();

        var errors = new List<string>();

        ValidateRequired(street, "Street", MaxStreetLength, errors);
        ValidateRequired(city, "City", MaxCityLength, errors);
        ValidateRequired(postalCode, "Postal code", MaxPostalCodeLength, errors);
        ValidateRequired(country, "Country", MaxCountryLength, errors);

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }

        return new Address(street, city, postalCode, country);
    }

    private static void ValidateRequired(string value, string fieldLabel, int maxLength, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"Address {fieldLabel.ToLowerInvariant()} is required.");
        }
        else if (value.Length > maxLength)
        {
            errors.Add($"Address {fieldLabel.ToLowerInvariant()} must not exceed {maxLength} characters.");
        }
    }

    public override string ToString() => $"{Street}, {City}, {PostalCode}, {Country}";
}
