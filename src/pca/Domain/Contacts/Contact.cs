using pca.Domain.Common;
using pca.Domain.Contacts.ValueObjects;

namespace pca.Domain.Contacts;

/// <summary>
/// Contact entity (aggregate root). There are no public setters - state can only be
/// changed through the intention-revealing Update* methods below, and a new
/// instance can only be created through <see cref="ContactBuilder"/>
/// (see <see cref="CreateBuilder"/>), which validates every field together.
/// </summary>
public sealed class Contact : Entity
{
    // internal (not private) so the Application layer's FluentValidation
    // rules can reuse this instead of redeclaring the same bound.
    internal const int MaxNameLength = 100;

    public const string FirstNameRequiredMessage = "First name is required.";
    public const string LastNameRequiredMessage = "Last name is required.";

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public Address Address { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Iban Iban { get; private set; }

    internal Contact(
        Guid id,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        Address address,
        PhoneNumber phoneNumber,
        Iban iban)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Iban = iban ?? throw new ArgumentNullException(nameof(iban));
    }

    public static ContactBuilder CreateBuilder() => new();

    public void UpdateName(string firstName, string lastName)
    {
        var errors = new List<string>();
        var validatedFirstName = ValidateName(firstName, "First name", FirstNameRequiredMessage, errors);
        var validatedLastName = ValidateName(lastName, "Last name", LastNameRequiredMessage, errors);

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }

        FirstName = validatedFirstName;
        LastName = validatedLastName;
    }

    public void UpdateDateOfBirth(DateOnly dateOfBirth)
    {
        var errors = new List<string>();
        ValidateDateOfBirth(dateOfBirth, errors);

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }

        DateOfBirth = dateOfBirth;
    }

    public void UpdateAddress(Address address) =>
        Address = address ?? throw new ArgumentNullException(nameof(address));

    public void UpdatePhoneNumber(PhoneNumber phoneNumber) =>
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));

    public void UpdateIban(Iban iban) =>
        Iban = iban ?? throw new ArgumentNullException(nameof(iban));

    /// <summary>
    /// Validates and trims a name field, appending any error to
    /// <paramref name="errors"/> instead of throwing, so callers (namely
    /// <see cref="ContactBuilder"/>) can collect every field error at once.
    /// </summary>
    internal static string ValidateName(string? name, string fieldLabel, string requiredMessage, List<string> errors)
    {
        var trimmed = (name ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            errors.Add(requiredMessage);
        }
        else if (trimmed.Length > MaxNameLength)
        {
            errors.Add($"{fieldLabel} must not exceed {MaxNameLength} characters.");
        }

        return trimmed;
    }

    internal static void ValidateDateOfBirth(DateOnly dateOfBirth, List<string> errors)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (dateOfBirth > today)
        {
            errors.Add("Date of birth cannot be in the future.");
        }
        else if (dateOfBirth < today.AddYears(-130))
        {
            errors.Add("Date of birth is not a valid, realistic date.");
        }
    }
}
