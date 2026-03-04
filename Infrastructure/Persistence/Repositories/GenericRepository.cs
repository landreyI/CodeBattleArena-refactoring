using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CodeBattleArena.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity<Guid>
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, bool asNoTracking = false, CancellationToken ct = default)
        {
            if (asNoTracking)
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

            // FindAsync идеален для команд, так как ищет в локальном кеше контекста
            return await _dbSet.FindAsync(new object[] { id }, ct);
        }

        public async Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> spec, CancellationToken ct = default)
        {
            return await SpecificationEvaluator.Default
                .GetQuery(_dbSet.AsQueryable(), spec)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<TEntity>> GetListBySpecAsync(ISpecification<TEntity> spec, CancellationToken ct = default)
        {
            return await SpecificationEvaluator.Default
                .GetQuery(_dbSet.AsQueryable(), spec)
                .ToListAsync(ct);
        }

        public async Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken ct = default)
        {
            return await SpecificationEvaluator.Default
                .GetQuery(_dbSet.AsQueryable(), spec)
                .AnyAsync(ct);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await _dbSet.AnyAsync(predicate, ct);
        }
        public async Task<TEntity?> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate, 
            bool asNoTracking = false, 
            CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<IReadOnlyList<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            bool asNoTracking = false,
            CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            return await query.ToListAsync(ct);
        }

        public async Task AddAsync(TEntity entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
