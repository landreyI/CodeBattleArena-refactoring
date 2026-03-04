using System.Text.Json.Serialization;
using System.Text.Json;
using StackExchange.Redis;
using System.Threading.Tasks;
using CodeBattleArena.Server.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CodeBattleArena.Server.Services.Cache
{
    public class CacheService : ICacheService
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles, // циклы
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task SetAsync<T>(string key, T data, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            };

            _cache.Set(key, data, options);
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            var found = _cache.TryGetValue(key, out T? result);
            return Task.FromResult(found ? result : default);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        /*private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _db = connectionMultiplexer.GetDatabase();
        }

        public async Task SetAsync<T>(string key, T data, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            await _db.StringSetAsync(key, json, expiration ?? TimeSpan.FromMinutes(10));
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _db.StringGetAsync(key);
            return json.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }*/
    }
}
