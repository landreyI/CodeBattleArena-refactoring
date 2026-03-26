namespace CodeBattleArena.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public Dictionary<string, string[]> Errors { get; }

        public bool WasModified { get; protected set; }

        protected Result(bool isSuccess, Error error, Dictionary<string, string[]>? errors = null, bool wasModified = false)
        {
            IsSuccess = isSuccess;
            Error = error;
            Errors = errors ?? new Dictionary<string, string[]>();
            WasModified = wasModified;
        }

        public static Result Success() => new(true, Error.None, wasModified: true);
        public static Result Failure(Error error) => new(false, error, wasModified: false);
        public static Result Failure(Error error, Dictionary<string, string[]> errors) => new(false, error, errors, false);

        public Result WithoutModification()
        {
            WasModified = false;
            return this;
        }
    }

    public class Result<TValue> : Result
    {
        public TValue? Value { get; }

        protected Result(TValue? value, bool isSuccess, Error error, Dictionary<string, string[]>? errors = null, bool wasModified = false)
            : base(isSuccess, error, errors, wasModified)
        {
            Value = value;
        }

        public static Result<TValue> Success(TValue value) => new(value, true, Error.None, wasModified: true);

        public new static Result<TValue> Failure(Error error) => new(default, false, error, wasModified: false);
        public new static Result<TValue> Failure(Error error, Dictionary<string, string[]> errors) => new(default, false, error, errors, false);

        public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure)
            => IsSuccess ? onSuccess(Value!) : onFailure(Error);

        public new Result<TValue> WithoutModification()
        {
            base.WithoutModification();
            return this;
        }
    }
}