
using Ardalis.Specification;
using CodeBattleArena.Domain.Common;
using System.Linq.Expressions;

namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface IRepository<TEntity> where TEntity : BaseEntity<Guid>
    {
        Task<TEntity?> GetByIdAsync(Guid id, bool asNoTracking = false, CancellationToken ct = default);
        Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> spec, CancellationToken ct = default);
        Task<List<TEntity>> GetListBySpecAsync(ISpecification<TEntity> spec, CancellationToken ct = default);
        Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken ct = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, CancellationToken ct = default);
        Task<IReadOnlyList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, bool asNoTracking = false, CancellationToken ct = default);
        Task AddAsync(TEntity entity, CancellationToken ct = default);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}
