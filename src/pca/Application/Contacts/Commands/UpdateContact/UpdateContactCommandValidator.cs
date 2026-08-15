using FluentValidation;
using pca.Application.Common.Validators;

namespace pca.Application.Contacts.Commands.UpdateContact;

public sealed class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Contact).NotNull();
        RuleFor(x => x.Contact)
            .SetValidator(new ContactInputModelValidator())
            .When(x => x.Contact is not null);
    }
}
