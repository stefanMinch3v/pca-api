using MediatR;
using pca.Application.Common;
using pca.Application.Common.Extensions;
using pca.Application.Common.Interfaces;
using pca.Application.Contacts.InputModels;
using pca.Application.Contacts.OutputModels;
using pca.Domain.Common;
using pca.Domain.Contacts;

namespace pca.Application.Contacts.Commands.CreateContact;

public class CreateContactCommand : IRequest<Result<ContactDetailsOutputModel>>
{
    public ContactInputModel Contact { get; set; } = default!;

    internal class CreateContactCommandHandler(IApplicationDbContext dbContext)
        : IRequestHandler<CreateContactCommand, Result<ContactDetailsOutputModel>>
    {
        public async Task<Result<ContactDetailsOutputModel>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            Contact contact;

            try
            {
                contact = new ContactBuilder()
                    .WithFirstName(request.Contact.FirstName)
                    .WithLastName(request.Contact.LastName)
                    .WithDateOfBirth(request.Contact.DateOfBirth)
                    .WithAddress(request.Contact.Street, request.Contact.City, request.Contact.PostalCode, request.Contact.Country)
                    .WithPhoneNumber(request.Contact.PhoneNumber)
                    .WithIban(request.Contact.Iban)
                    .Build();
            }
            catch (DomainValidationException ex)
            {
                return Result<ContactDetailsOutputModel>.Failure(ex.Errors);
            }

            dbContext.Contacts.Add(contact);
            await dbContext.SaveChangesAsync(cancellationToken);

            return contact.ToDetailsOutputModel();
        }
    }
}
