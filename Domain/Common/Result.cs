
namespace CodeBattleArena.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        // Словарь для ошибок валидации
        public Dictionary<string, string[]> Errors { get; }

        protected Result(bool isSuccess, Error error, Dictionary<string, string[]>? errors = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            Errors = errors ?? new Dictionary<string, string[]>();
        }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
        public static Result Failure(Error error, Dictionary<string, string[]> errors) => new(false, error, errors);
    }
    public class Result<TValue> : Result
    {
        public TValue? Value { get; }

        protected Result(TValue? value, bool isSuccess, Error error, Dictionary<string, string[]>? errors = null) : base(isSuccess, error, errors)
        {
            Value = value;
        }

        public static Result<TValue> Success(TValue value) =>
            new(value, true, Error.None);

        public new static Result<TValue> Failure(Error error) =>
            new(default, false, error);

        public new static Result<TValue> Failure(Error error, Dictionary<string, string[]> errors) =>
            new(default, false, error, errors);

        public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure)
            => IsSuccess ? onSuccess(Value!) : onFailure(Error);
    }
}
