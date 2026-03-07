using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Infrastructure.Persistence.Redis;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;


namespace CodeBattleArena.Infrastructure.DI
{
    public static class CachingDI
    {
        public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
        {
            var redisConnection = config.GetConnectionString("Redis") ?? "localhost:6379";

            // Общий мультиплексор
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));

            services.AddScoped<ICacheService, RedisCacheService>();

            // L2 уровень для HybridCache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "CodeBattleArena_";
            });

            services.AddHybridCache(options =>
            {
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(10),
                    LocalCacheExpiration = TimeSpan.FromMinutes(2)
                };
            });

            return services;
        }
    }
}
