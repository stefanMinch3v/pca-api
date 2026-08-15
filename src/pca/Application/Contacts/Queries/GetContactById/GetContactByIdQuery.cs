using MediatR;
using pca.Application.Common;
using pca.Application.Common.Extensions;
using pca.Application.Contacts.OutputModels;

namespace pca.Application.Contacts.Queries.GetContactById;

public class GetContactByIdQuery : IRequest<Result<ContactDetailsOutputModel>>
{
    public Guid Id { get; set; }

    internal class GetContactByIdQueryHandler(IContactRepository contactRepository)
        : IRequestHandler<GetContactByIdQuery, Result<ContactDetailsOutputModel>>
    {
        public async Task<Result<ContactDetailsOutputModel>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            var contact = await contactRepository.GetByIdAsync(request.Id, cancellationToken);

            return contact is null
                ? Result<ContactDetailsOutputModel>.NotFound($"Contact with id '{request.Id}' was not found.")
                : contact.ToDetailsOutputModel();
        }
    }
}
