using CodeBattleArena.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http.Json;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace CodeBattleArena.Infrastructure.Persistence.Redis
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly JsonSerializerOptions _options;

        public RedisCacheService(IConnectionMultiplexer redis, IOptions<JsonOptions> jsonOptions)
        {
            _db = redis.GetDatabase();

            _options = jsonOptions.Value.SerializerOptions;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var value = await _db.StringGetAsync(key);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value!, _options);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(value, _options);

            await _db.StringSetAsync(key, json, expiry, keepTtl: false);
        }

        public async Task<bool> RemoveAsync(string key, CancellationToken ct = default)
            => await _db.KeyDeleteAsync(key);

        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
            => await _db.KeyExistsAsync(key);

        public async Task<long> IncrementAsync(string key, string field, long value = 1, CancellationToken ct = default)
            => await _db.HashIncrementAsync(key, field, value);

        public async Task<long> DecrementAsync(string key, string field, long value = 1, CancellationToken ct = default)
            => await _db.HashDecrementAsync(key, field, value);

        public async Task<bool> AddToSetAsync(string key, string value, CancellationToken ct = default)
            => await _db.SetAddAsync(key, value);

        public async Task<bool> RemoveFromSetAsync(string key, string value, CancellationToken ct = default)
            => await _db.SetRemoveAsync(key, value);

        public async Task<long> GetSetCountAsync(string key, CancellationToken ct = default)
            => await _db.SetLengthAsync(key);
    }
}
