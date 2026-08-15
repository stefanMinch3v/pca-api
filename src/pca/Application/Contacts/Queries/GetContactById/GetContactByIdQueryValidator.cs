using FluentValidation;

namespace pca.Application.Contacts.Queries.GetContactById;

public sealed class GetContactByIdQueryValidator : AbstractValidator<GetContactByIdQuery>
{
    public GetContactByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
