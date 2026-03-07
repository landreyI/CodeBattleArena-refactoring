using CodeBattleArena.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;

namespace CodeBattleArena.Application.Common.Behaviours
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICachableRequest
    {
        private readonly HybridCache _hybridCache;

        public CachingBehavior(HybridCache hybridCache)
        {
            _hybridCache = hybridCache;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            // Настраиваем опции для конкретного запроса
            var options = new HybridCacheEntryOptions
            {
                Expiration = request.Expiration ?? TimeSpan.FromMinutes(5),
            };

            // GetOrCreateAsync берет на себя ВСЮ логику:
            // 1. Проверка L1 (Memory)
            // 2. Проверка L2 (Redis)
            // 3. Если пусто -> Блокировка (Stampede Protection) -> Вызов next()
            // 4. Сохранение результата в L1 и L2
            return await _hybridCache.GetOrCreateAsync(
                request.CacheKey,
                async token => await next(),
                options,
                request.Tags,
                cancellationToken: ct
            );
        }
    }
}