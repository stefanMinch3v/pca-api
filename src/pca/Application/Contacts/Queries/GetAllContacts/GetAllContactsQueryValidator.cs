using FluentValidation;

namespace pca.Application.Contacts.Queries.GetAllContacts;

public sealed class GetAllContactsQueryValidator : AbstractValidator<GetAllContactsQuery>
{
    // A malformed/tampered page key just falls back to decoding as "no
    // cursor" (see PageKeyEncoder.Decode), so there's nothing to validate
    // about its shape here - only a sane upper bound on its length.
    private const int MaxPageKeyLength = 2048;

    public GetAllContactsQueryValidator()
    {
        RuleFor(x => x.PageKey).MaximumLength(MaxPageKeyLength);
    }
}
