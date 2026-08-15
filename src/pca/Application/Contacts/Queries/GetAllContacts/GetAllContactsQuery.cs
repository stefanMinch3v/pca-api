using MediatR;
using pca.Application.Common;
using pca.Application.Common.Extensions;
using pca.Application.Contacts.OutputModels;

namespace pca.Application.Contacts.Queries.GetAllContacts;

public class GetAllContactsQuery : IRequest<Result<IReadOnlyList<ContactListingOutputModel>>>
{
    internal class GetAllContactsQueryHandler(IContactRepository contactRepository)
        : IRequestHandler<GetAllContactsQuery, Result<IReadOnlyList<ContactListingOutputModel>>>
    {
        public async Task<Result<IReadOnlyList<ContactListingOutputModel>>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
        {
            var contacts = await contactRepository.GetAllAsync(cancellationToken);

            IReadOnlyList<ContactListingOutputModel> listing = contacts
                .Select(contact => contact.ToListingOutputModel())
                .ToList();

            return Result<IReadOnlyList<ContactListingOutputModel>>.SuccessWith(listing);
        }
    }
}
