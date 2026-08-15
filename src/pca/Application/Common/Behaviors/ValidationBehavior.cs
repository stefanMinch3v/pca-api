using FluentValidation;
using MediatR;

namespace pca.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs every registered FluentValidation
/// <see cref="IValidator{T}"/> for the incoming request before it reaches its
/// handler. On failure it short-circuits by returning <c>TResponse.Failure(...)</c>
/// instead of throwing - every command/query in this codebase returns
/// <see cref="Result"/> or <see cref="Result{TData}"/>, so this stays fully
/// type-safe via the <see cref="IFailureResult{TSelf}"/> static abstract member,
/// with no reflection involved. Requests with no registered validators pass
/// straight through.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result, IFailureResult<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .Select(failure => $"{failure.PropertyName}: {failure.ErrorMessage}")
            .ToList();

        if (failures.Count > 0)
        {
            return TResponse.Failure(failures);
        }

        return await next(cancellationToken);
    }
}
