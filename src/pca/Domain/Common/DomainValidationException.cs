namespace pca.Domain.Common;

/// <summary>
/// Raised when one or more domain invariants are violated while creating or
/// mutating a domain object. Carries every violation found, rather than just
/// the first one, so builders can report all field errors together.
/// </summary>
public sealed class DomainValidationException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }

    public DomainValidationException(IReadOnlyCollection<string> errors)
        : base(BuildMessage(errors))
    {
        Errors = errors;
    }

    public DomainValidationException(string error)
        : this([error])
    {
    }

    private static string BuildMessage(IReadOnlyCollection<string> errors) =>
        $"Domain validation failed with {errors.Count} error(s): {string.Join("; ", errors)}";
}
