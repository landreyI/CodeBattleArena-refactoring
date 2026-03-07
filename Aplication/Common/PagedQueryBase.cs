using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Common
{
    public abstract record PagedQueryBase<T> : IRequest<Result<PaginatedResult<T>>>, ICachableRequest
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 15;

        // для ICachableRequest
        public abstract string CacheKey { get; }
        public virtual string[] Tags => [];
        public virtual TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }
}
