namespace pca.Application.Common
{
    public enum ResultStatus
    {
        Ok,
        Invalid,
        NotFound
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
        private readonly IEnumerable<string> errors;

        internal Result(bool succeeded, ResultStatus status, IEnumerable<string> errors)
        {
            this.Succeeded = succeeded;
            this.Status = status;
            this.errors = errors;
        }

        public bool Succeeded { get; }

        public ResultStatus Status { get; }

        public IEnumerable<string> Errors
            => this.Succeeded
                ? Enumerable.Empty<string>()
                : this.errors;

        public static Result Success
            => new(true, ResultStatus.Ok, Enumerable.Empty<string>());

        public static Result Failure(IEnumerable<string> errors)
            => new(false, ResultStatus.Invalid, errors);

        public static Result NotFound(string error)
            => new(false, ResultStatus.NotFound, [error]);

        public static implicit operator Result(string error)
            => Failure(new[] { error });

        public static implicit operator Result(string[] errors)
            => Failure(errors);

        public static implicit operator Result(List<string> errors)
            => Failure(errors);

        public static implicit operator Result(bool success)
            => success ? Success : Failure(Enumerable.Empty<string>());
    }

    public class Result<TData> : Result, IFailureResult<Result<TData>>
    {
        private readonly TData? data;

        private Result(bool succeeded, ResultStatus status, TData? data, IEnumerable<string> errors)
            : base(succeeded, status, errors)
            => this.data = data;

        public TData Data
            => this.Succeeded
                ? this.data!
                : throw new InvalidOperationException(
                    $"{nameof(this.Data)} is not available with a failed result. Use {nameof(this.Errors)} instead.");

        public static Result<TData> SuccessWith(TData data)
            => new(true, ResultStatus.Ok, data, Enumerable.Empty<string>());

        public new static Result<TData> Failure(IEnumerable<string> errors)
            => new(false, ResultStatus.Invalid, default, errors);

        public new static Result<TData> NotFound(string error)
            => new(false, ResultStatus.NotFound, default, [error]);

        public static implicit operator Result<TData>(TData data)
            => SuccessWith(data);

        public static implicit operator Result<TData>(string error)
            => Failure(new[] { error });

        public static implicit operator Result<TData>(string[] errors)
            => Failure(errors);

        public static implicit operator Result<TData>(List<string> errors)
            => Failure(errors);
    }
}
