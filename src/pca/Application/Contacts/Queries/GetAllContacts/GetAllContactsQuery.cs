using MediatR;
using Microsoft.EntityFrameworkCore;
using pca.Application.Common;
using pca.Application.Common.Extensions;
using pca.Application.Common.Interfaces;
using pca.Application.Contacts.OutputModels;

namespace pca.Application.Contacts.Queries.GetAllContacts;

public class GetAllContactsQuery : IRequest<Result<IReadOnlyList<ContactListingOutputModel>>>
{
    internal class GetAllContactsQueryHandler(IApplicationDbContext dbContext)
        : IRequestHandler<GetAllContactsQuery, Result<IReadOnlyList<ContactListingOutputModel>>>
    {
        public async Task<Result<IReadOnlyList<ContactListingOutputModel>>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
        {
            var contacts = await dbContext.Contacts.AsNoTracking().ToListAsync(cancellationToken);

            IReadOnlyList<ContactListingOutputModel> listing = contacts
                .Select(contact => contact.ToListingOutputModel())
                .ToList();

            return Result<IReadOnlyList<ContactListingOutputModel>>.SuccessWith(listing);
        }
    }
}
