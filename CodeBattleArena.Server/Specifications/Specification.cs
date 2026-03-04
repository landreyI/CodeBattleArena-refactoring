using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace CodeBattleArena.Server.Specifications
{
    public abstract class Specification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; protected set; }
        public List<Expression<Func<T, object>>> Includes { get; protected set; } = new();
        public Expression<Func<T, object>> OrderBy { get; protected set; }
        public Expression<Func<T, object>> OrderByDescending { get; protected set; }
        public int? Take { get; protected set; }
        public int? Skip { get; protected set; }
        public IFilter<T>? Filter { get; protected set; }

        public void AddInclude(Expression<Func<T, object>> include) => Includes.Add(include);
        public void ApplyOrderBy(Expression<Func<T, object>> orderBy) => OrderBy = orderBy;
        public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescending) => OrderByDescending = orderByDescending;
        public void ApplyTake(int take) => Take = take;
        public void ApplySkip(int skip) => Skip = skip;
        public void ApplyFilter(IFilter<T> filter) => Filter = filter;


        public virtual string? GetCacheKey()
        {
            return null;
        }

        public static Specification<T> Combine(params Specification<T>[] specifications)
        {
            var combined = new CombinedSpecification<T>();
            Expression<Func<T, bool>> combinedCriteria = null;
            var parameter = Expression.Parameter(typeof(T), "pt");

            foreach (var spec in specifications)
            {
                if (spec.Criteria != null)
                {
                    var reboundBody = new ParameterRebinder(spec.Criteria.Parameters[0], parameter).Visit(spec.Criteria.Body);

                    combinedCriteria = combinedCriteria == null
                        ? Expression.Lambda<Func<T, bool>>(reboundBody, parameter)
                        : Expression.Lambda<Func<T, bool>>(
                            Expression.AndAlso(
                                new ParameterRebinder(combinedCriteria.Parameters[0], parameter).Visit(combinedCriteria.Body),
                                reboundBody),
                            parameter);
                }
                combined.Includes.AddRange(spec.Includes);
            }

            combined.Criteria = combinedCriteria ?? (x => true);
            return combined;
        }

        // Класс для замены параметров
        public class ParameterRebinder : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam;
            private readonly ParameterExpression _newParam;

            public ParameterRebinder(ParameterExpression oldParam, ParameterExpression newParam)
            {
                _oldParam = oldParam;
                _newParam = newParam;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }
    }

    public class CombinedSpecification<T> : Specification<T> { }
}
