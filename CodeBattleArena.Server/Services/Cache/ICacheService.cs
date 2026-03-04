namespace CodeBattleArena.Server.Services.Cache
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T data, TimeSpan? expiration = null);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
