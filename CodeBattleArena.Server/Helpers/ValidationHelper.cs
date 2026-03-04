using CodeBattleArena.Server.Untils;

namespace CodeBattleArena.Server.Helpers
{
    public static class ValidationHelper
    {
        public static Result<T, ErrorResponse> CheckUserId<T>(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure<T, ErrorResponse>(new ErrorResponse { Error = "Player not found." });

            return Result.Success<T, ErrorResponse>(default!); // default! — потому что T будет проигнорирован в этом случае
        }
    }
}
