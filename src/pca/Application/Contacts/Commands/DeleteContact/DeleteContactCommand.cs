using MediatR;
using pca.Application.Common;

namespace pca.Application.Contacts.Commands.DeleteContact;

public class DeleteContactCommand : IRequest<Result>
{
    public Guid Id { get; set; }

    internal class DeleteContactCommandHandler(IContactRepository contactRepository)
        : IRequestHandler<DeleteContactCommand, Result>
    {
        public async Task<Result> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await contactRepository.GetByIdAsync(request.Id, cancellationToken);
            if (contact is null)
            {
                return Result.NotFound($"Contact with id '{request.Id}' was not found.");
            }

            await contactRepository.DeleteAsync(contact, cancellationToken);

            return Result.Success;
        }
    }
}
