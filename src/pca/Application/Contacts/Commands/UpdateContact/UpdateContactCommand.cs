using MediatR;
using Microsoft.EntityFrameworkCore;
using pca.Application.Common;
using pca.Application.Common.Extensions;
using pca.Application.Common.Interfaces;
using pca.Application.Contacts.InputModels;
using pca.Application.Contacts.OutputModels;
using pca.Domain.Common;
using pca.Domain.Contacts;

namespace pca.Application.Contacts.Commands.UpdateContact;

public class UpdateContactCommand : IRequest<Result<ContactDetailsOutputModel>>
{
    public Guid Id { get; set; }

    public ContactInputModel Contact { get; set; } = default!;

    internal class UpdateContactCommandHandler(IApplicationDbContext dbContext)
        : IRequestHandler<UpdateContactCommand, Result<ContactDetailsOutputModel>>
    {
        public async Task<Result<ContactDetailsOutputModel>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (contact is null)
            {
                return Result<ContactDetailsOutputModel>.NotFound($"Contact with id '{request.Id}' was not found.");
            }

            try
            {
                // Mutates the same tracked instance in place, so EF's change
                // tracker picks up the update - no explicit Update() call needed.
                new ContactBuilder()
                    .WithFirstName(request.Contact.FirstName)
                    .WithLastName(request.Contact.LastName)
                    .WithDateOfBirth(request.Contact.DateOfBirth)
                    .WithAddress(request.Contact.Street, request.Contact.City, request.Contact.PostalCode, request.Contact.Country)
                    .WithPhoneNumber(request.Contact.PhoneNumber)
                    .WithIban(request.Contact.Iban)
                    .ApplyTo(contact);
            }
            catch (DomainValidationException ex)
            {
                return Result<ContactDetailsOutputModel>.Failure(ex.Errors);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return contact.ToDetailsOutputModel();
        }
    }
}
