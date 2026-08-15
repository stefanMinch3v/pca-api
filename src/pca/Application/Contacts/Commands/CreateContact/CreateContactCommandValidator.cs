using FluentValidation;
using pca.Application.Common.Validators;

namespace pca.Application.Contacts.Commands.CreateContact;

public sealed class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator()
    {
        RuleFor(x => x.Contact).NotNull();
        RuleFor(x => x.Contact)
            .SetValidator(new ContactInputModelValidator())
            .When(x => x.Contact is not null);
    }
}
