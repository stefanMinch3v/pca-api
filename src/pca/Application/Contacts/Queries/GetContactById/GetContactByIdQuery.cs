using MediatR;
using Microsoft.EntityFrameworkCore;
using pca.Application.Common;
using pca.Application.Common.DTOs;
using pca.Application.Common.Interfaces;
using pca.Application.Contacts.OutputModels;

namespace pca.Application.Contacts.Queries.GetContactById;

public class GetContactByIdQuery : IRequest<Result<ContactDetailsOutputModel>>
{
    public Guid Id { get; set; }

    internal class GetContactByIdQueryHandler(IApplicationDbContext dbContext)
        : IRequestHandler<GetContactByIdQuery, Result<ContactDetailsOutputModel>>
    {
        public async Task<Result<ContactDetailsOutputModel>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            // Projects straight into the output model instead of
            // materializing a full Contact entity and mapping in memory
            // afterwards, so EF Core can skip the audit columns (CreatedAt/
            // UpdatedAt) that the output model never uses.
            var contact = await dbContext.Contacts
                .AsNoTracking()
                .Where(c => c.Id == request.Id)
                .Select(c => new ContactDetailsOutputModel(
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.DateOfBirth,
                    new AddressDto(c.Address.Street, c.Address.City, c.Address.PostalCode, c.Address.Country),
                    c.PhoneNumber.Value,
                    c.Iban.Value))
                .FirstOrDefaultAsync(cancellationToken);

            return contact is null
                ? Result<ContactDetailsOutputModel>.Failure($"Contact with id '{request.Id}' was not found.")
                : contact;
        }
    }
}
