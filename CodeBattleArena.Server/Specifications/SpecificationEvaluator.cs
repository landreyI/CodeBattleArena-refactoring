using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Server.Specifications
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> spec)
            where T : class
        {
            var query = inputQuery;

            foreach (var include in spec.Includes.Distinct())
            {
                query = query.Include(include);
            }

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            if(spec.Filter != null)
                query = spec.Filter.ApplyTo(query);

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);

            if(spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if(spec.Take.HasValue)
                query = query.Take(spec.Take.Value);
            
            if(spec.Skip.HasValue)
                query = query.Skip(spec.Skip.Value);

            return query;
        }

        public static IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> inputQuery, IProjectableSpecification<T, TResult> spec)
             where T : class
        {
            var query = GetQuery(inputQuery, (ISpecification<T>)spec);
            return query.Select(spec.Select);
        }
    }
}
