
namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
        Task<bool> RemoveAsync(string key, CancellationToken ct = default);
        Task<bool> ExistsAsync(string key, CancellationToken ct = default);

        Task<long> IncrementAsync(string key, string field, long value = 1, CancellationToken ct = default);
        Task<long> DecrementAsync(string key, string field, long value = 1, CancellationToken ct = default);

        Task<bool> AddToSetAsync(string key, string value, CancellationToken ct = default);
        Task<bool> RemoveFromSetAsync(string key, string value, CancellationToken ct = default);
        Task<long> GetSetCountAsync(string key, CancellationToken ct = default);
    }
}
