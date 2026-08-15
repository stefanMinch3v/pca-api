using pca.Domain.Common;
using pca.Domain.Contacts.ValueObjects;

namespace pca.Domain.Contacts;

/// <summary>
/// Fluent builder for <see cref="Contact"/>. Each With* method tries to
/// build its corresponding primitive/value object and records any failure
/// instead of throwing immediately, so <see cref="Build"/> can report every
/// field error together in a single <see cref="DomainValidationException"/>.
/// </summary>
public sealed class ContactBuilder
{
    public const string DateOfBirthRequiredMessage = "Date of birth is required.";
    public const string AddressRequiredMessage = "Address is required.";
    public const string PhoneNumberRequiredMessage = "Phone number is required.";

    private readonly List<string> _errors = [];

    private string? _firstName;
    private string? _lastName;
    private DateOnly? _dateOfBirth;
    private Address? _address;
    private PhoneNumber? _phoneNumber;
    private Iban? _iban;

    public ContactBuilder WithFirstName(string firstName)
    {
        _firstName = Contact.ValidateName(firstName, "First name", Contact.FirstNameRequiredMessage, _errors);
        return this;
    }

    public ContactBuilder WithLastName(string lastName)
    {
        _lastName = Contact.ValidateName(lastName, "Last name", Contact.LastNameRequiredMessage, _errors);
        return this;
    }

    public ContactBuilder WithDateOfBirth(DateOnly dateOfBirth)
    {
        Contact.ValidateDateOfBirth(dateOfBirth, _errors);
        _dateOfBirth = dateOfBirth;
        return this;
    }

    public ContactBuilder WithAddress(string street, string city, string postalCode, string country)
    {
        _address = TryCreate(() => Address.Create(street, city, postalCode, country));
        return this;
    }

    public ContactBuilder WithPhoneNumber(string phoneNumber)
    {
        _phoneNumber = TryCreate(() => PhoneNumber.Create(phoneNumber));
        return this;
    }

    public ContactBuilder WithIban(string iban)
    {
        _iban = TryCreate(() => Iban.Create(iban));
        return this;
    }

    /// <summary>
    /// Validates that every required field was supplied and is valid, and
    /// either returns a fully-formed <see cref="Contact"/> or throws a
    /// <see cref="DomainValidationException"/> listing every error found.
    /// </summary>
    public Contact Build()
    {
        if (_firstName is null)
        {
            _errors.Add(Contact.FirstNameRequiredMessage);
        }

        if (_lastName is null)
        {
            _errors.Add(Contact.LastNameRequiredMessage);
        }

        if (_dateOfBirth is null)
        {
            _errors.Add(DateOfBirthRequiredMessage);
        }

        if (_address is null)
        {
            _errors.Add(AddressRequiredMessage);
        }

        if (_phoneNumber is null)
        {
            _errors.Add(PhoneNumberRequiredMessage);
        }

        if (_iban is null)
        {
            _errors.Add(Iban.RequiredErrorMessage);
        }

        if (_errors.Count > 0)
        {
            throw new DomainValidationException([.. _errors.Distinct()]);
        }

        return new Contact(
            Guid.CreateVersion7(),
            _firstName!,
            _lastName!,
            _dateOfBirth!.Value,
            _address!,
            _phoneNumber!,
            _iban!);
    }

    private T? TryCreate<T>(Func<T> func) where T : class
    {
        try
        {
            return func();
        }
        catch (DomainValidationException ex)
        {
            _errors.AddRange(ex.Errors);
            return null;
        }
    }
}
