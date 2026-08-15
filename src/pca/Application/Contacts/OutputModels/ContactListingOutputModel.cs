namespace pca.Application.Contacts.OutputModels;

/// <summary>
/// Deliberately leaner than <see cref="ContactDetailsOutputModel"/>: a bulk
/// "list all contacts" response has no need to carry every field (address,
/// date of birth) - and shouldn't carry the IBAN at all - just to render a
/// list row. Full details are fetched per-contact via GetContactById.
/// </summary>
public sealed record ContactListingOutputModel(
    Guid Id,
    string FirstName,
    string LastName,
    string PhoneNumber);
