using pca.Application.Common.DTOs;

namespace pca.Application.Contacts.InputModels
{
    public sealed record ContactInputModel(
         string FirstName,
         string LastName,
         DateOnly DateOfBirth,
         string Country,
         string Street,
         string PostalCode,
         string City,
         string PhoneNumber,
         string Iban);
}
