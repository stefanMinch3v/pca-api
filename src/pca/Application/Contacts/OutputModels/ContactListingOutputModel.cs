namespace pca.Application.Contacts.OutputModels;

public sealed record ContactListingOutputModel(
    Guid Id,
    string FirstName,
    string LastName,
    string PhoneNumber);
