using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;

namespace CodeBattleArena.Application.Common.Behaviours
{
    public class InvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheInvalidator
    {
        private readonly HybridCache _cache;

        public InvalidationBehavior(HybridCache cache) => _cache = cache;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            var response = await next();

            // Проверяем, успешно ли прошла команда
            // Мы проверяем свойство IsSuccess через рефлексию или паттерн-матчинг
            if (response is Result { IsSuccess: true, WasModified: true })
            {
                foreach (var key in request.CacheKeys) await _cache.RemoveAsync(key, ct);
                foreach (var tag in request.Tags) await _cache.RemoveByTagAsync(tag, ct);
            }

            return response;
        }
    }
}
