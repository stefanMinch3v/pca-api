using pca.Application.Common.DTOs;

namespace pca.Application.Contacts.OutputModels;

public sealed record ContactDetailsOutputModel(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    AddressDto Address,
    string PhoneNumber,
    string Iban);
