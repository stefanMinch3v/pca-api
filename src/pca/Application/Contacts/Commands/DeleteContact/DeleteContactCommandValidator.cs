using FluentValidation;

namespace pca.Application.Contacts.Commands.DeleteContact;

public sealed class DeleteContactCommandValidator : AbstractValidator<DeleteContactCommand>
{
    public DeleteContactCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
