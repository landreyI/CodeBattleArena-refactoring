
using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Application.Common.Helpers
{
    public static class ValidationHelper
    {
        public static Result<T> CheckIdentityId<T>(string? userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<T>.Failure(new Error(
                    "auth.user_not_found",
                    "Player identification failed.",
                    401));
            }

            return Result<T>.Success(default!);
        }

        public static Result<T> CheckPlayerId<T>(Guid? playerId)
        {
            if (playerId == Guid.Empty)
            {
                return Result<T>.Failure(new Error(
                    "player.player_not_found",
                    "Player ID required.",
                    401));
            }

            return Result<T>.Success(default!);
        }
    }
}
