using FluentValidation;
using pca.Application.Contacts.InputModels;
using pca.Domain.Contacts;
using pca.Domain.Contacts.ValueObjects;

namespace pca.Application.Common.Validators;

/// <summary>
/// Application-boundary validation shared by create/update commands (via
/// composition - see each command's own validator): required-ness and
/// length bounds only, so obviously malformed input fails fast with clean,
/// field-level messages before it reaches the Domain layer. The numeric
/// bounds are reused from the Domain value objects (not redeclared here) so
/// there is a single source of truth for them.
///
/// Deliberately NOT validated here: anything that requires domain
/// knowledge to judge (IBAN checksum, phone number format, a realistic date
/// of birth). Those are business rules, not input shape, and duplicating
/// them here would risk drifting out of sync with <c>ContactBuilder</c>,
/// which already owns and enforces them.
/// </summary>
public sealed class ContactInputModelValidator : AbstractValidator<ContactInputModel>
{
    public ContactInputModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(Contact.MaxNameLength);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(Contact.MaxNameLength);

        RuleFor(x => x.Street).NotEmpty().MaximumLength(Address.MaxStreetLength);
        RuleFor(x => x.City).NotEmpty().MaximumLength(Address.MaxCityLength);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(Address.MaxPostalCodeLength);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(Address.MaxCountryLength);

        RuleFor(x => x.PhoneNumber).NotEmpty().Length(PhoneNumber.MinLength, PhoneNumber.MaxLength);
        RuleFor(x => x.Iban).NotEmpty().Length(Iban.MinLength, Iban.MaxLength);
    }
}
