
namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface ICacheInvalidator
    {
        string[] CacheKeys => [];

        string[] Tags => [];
    }
}
