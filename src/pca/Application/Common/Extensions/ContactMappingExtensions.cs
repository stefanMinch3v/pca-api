using pca.Application.Common.DTOs;
using pca.Application.Contacts.OutputModels;
using pca.Domain.Contacts;

namespace pca.Application.Common.Extensions;

public static class ContactMappingExtensions
{
    public static ContactDetailsOutputModel ToDetailsOutputModel(this Contact contact) =>
        new(
            contact.Id,
            contact.FirstName,
            contact.LastName,
            contact.DateOfBirth,
            new AddressDto(
                contact.Address.Street,
                contact.Address.City,
                contact.Address.PostalCode,
                contact.Address.Country),
            contact.PhoneNumber.Value,
            contact.Iban.Value);

    public static ContactListingOutputModel ToListingOutputModel(this Contact contact) =>
        new(
            contact.Id,
            contact.FirstName,
            contact.LastName,
            contact.PhoneNumber.Value);
}
