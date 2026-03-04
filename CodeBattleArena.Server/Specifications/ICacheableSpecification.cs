namespace CodeBattleArena.Server.Specifications
{
    public interface ICacheableSpecification<T>
    {
        string? GetCacheKey();
    }
}
