using MediatR;
using Microsoft.EntityFrameworkCore;
using pca.Application.Common;
using pca.Application.Common.Interfaces;

namespace pca.Application.Contacts.Commands.DeleteContact;

public class DeleteContactCommand : IRequest<Result>
{
    public Guid Id { get; set; }

    internal class DeleteContactCommandHandler(IApplicationDbContext dbContext)
        : IRequestHandler<DeleteContactCommand, Result>
    {
        public async Task<Result> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (contact is null)
            {
                return Result.Failure($"Contact with id '{request.Id}' was not found.");
            }

            dbContext.Contacts.Remove(contact);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
