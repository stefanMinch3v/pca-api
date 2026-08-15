namespace pca.Application.Common.DTOs;

public sealed record AddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country);