
namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface ICachableRequest
    {
        string CacheKey { get; }
        TimeSpan? Expiration { get; }
        string[] Tags => Array.Empty<string>();
    }
}
