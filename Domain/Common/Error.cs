
namespace CodeBattleArena.Domain.Common
{
    public record Error(string Code, string Message, int StatusCode = 400)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("error.null", "Value cannot be null", 400);
    }
}
