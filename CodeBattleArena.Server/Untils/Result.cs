namespace CodeBattleArena.Server.Untils
{
    public class Result<TSuccess, TFailure>
    {
        public bool IsSuccess { get; }
        public TSuccess? Success { get; }
        public TFailure? Failure { get; }

        protected Result(bool isSuccess, TSuccess? success, TFailure? failure)
        {
            IsSuccess = isSuccess;
            Success = success;
            Failure = failure;
        }

        public static Result<TSuccess, TFailure> SuccessResult(TSuccess success)
            => new(true, success, default);

        public static Result<TSuccess, TFailure> FailureResult(TFailure failure)
            => new(false, default, failure);

        public TResult Match<TResult>(Func<TSuccess, TResult> onSuccess, Func<TFailure, TResult> onFailure)
            => IsSuccess ? onSuccess(Success!) : onFailure(Failure!);
    }

    public static class Result
    {
        public static Result<TSuccess, TFailure> Success<TSuccess, TFailure>(TSuccess value)
            => Result<TSuccess, TFailure>.SuccessResult(value);

        public static Result<TSuccess, TFailure> Failure<TSuccess, TFailure>(TFailure error)
            => Result<TSuccess, TFailure>.FailureResult(error);
    }

}
