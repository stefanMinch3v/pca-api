namespace pca.Application.Common
{
    /// <summary>
    /// A bag of human-readable messages describing why an operation failed.
    /// Deliberately carries no HTTP semantics (no "NotFound"/"Conflict" kind,
    /// no status code) - this layer only knows Success/Failure. Mapping a
    /// failure onto an HTTP response (status code, ProblemDetails, or a
    /// plain 404) is entirely the API layer's job.
    /// </summary>
    public sealed record Error(IReadOnlyList<string> Messages)
    {
        public static readonly Error None = new([]);
    }

    /// <summary>
    /// Lets <see cref="Behaviors.ValidationBehavior{TRequest, TResponse}"/> short-circuit
    /// generically (no reflection) into whichever concrete <see cref="Result"/>/
    /// <see cref="Result{TData}"/> a given command/query returns, via C# static
    /// abstract interface members.
    /// </summary>
    public interface IFailureResult<out TSelf> where TSelf : Result
    {
        static abstract TSelf Failure(IEnumerable<string> errors);
    }

    public class Result : IFailureResult<Result>
    {
        internal Result(bool succeeded, Error error)
        {
            this.Succeeded = succeeded;
            this.Error = error;
        }

        public bool Succeeded { get; }

        public Error Error { get; }

        public static Result Success
            => new(true, Error.None);

        public static Result Failure(string error)
            => Failure([error]);

        public static Result Failure(IEnumerable<string> errors)
            => new(false, new Error(errors.ToList()));

        public static implicit operator Result(string error)
            => Failure(error);

        public static implicit operator Result(string[] errors)
            => Failure(errors);

        public static implicit operator Result(List<string> errors)
            => Failure(errors);

        public static implicit operator Result(bool success)
            => success ? Success : Failure([]);
    }

    public class Result<TData> : Result, IFailureResult<Result<TData>>
    {
        private readonly TData? value;

        private Result(bool succeeded, Error error, TData? value)
            : base(succeeded, error)
            => this.value = value;

        public TData Value
            => this.Succeeded
                ? this.value!
                : throw new InvalidOperationException(
                    $"{nameof(this.Value)} is not available with a failed result. Use {nameof(this.Error)} instead.");

        public static Result<TData> SuccessWith(TData value)
            => new(true, Error.None, value);

        public new static Result<TData> Failure(string error)
            => Failure([error]);

        public new static Result<TData> Failure(IEnumerable<string> errors)
            => new(false, new Error(errors.ToList()), default);

        public static implicit operator Result<TData>(TData value)
            => SuccessWith(value);

        public static implicit operator Result<TData>(string error)
            => Failure(error);

        public static implicit operator Result<TData>(string[] errors)
            => Failure(errors);

        public static implicit operator Result<TData>(List<string> errors)
            => Failure(errors);
    }
}
