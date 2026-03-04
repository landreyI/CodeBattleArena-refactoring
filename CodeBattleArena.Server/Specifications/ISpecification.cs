using CodeBattleArena.Server.Filters;
using System.Linq.Expressions;

namespace CodeBattleArena.Server.Specifications
{
    public interface ISpecification<T> 
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        int? Take { get; }
        int? Skip { get; }
        IFilter<T>? Filter { get; }
        string? GetCacheKey();
    }
    public interface IProjectableSpecification<T, TResult> : ISpecification<T>
    {
        Expression<Func<T, TResult>> Select { get; }
    }
}
