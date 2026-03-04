using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications.SessionSpec;
using System.Linq.Expressions;

namespace CodeBattleArena.Server.Specifications.QuestSpec
{
    public class TaskPlaySpec : TaskPlayDefaultIncludesSpec
    {
        private readonly int? _id;
        public TaskPlaySpec(int ?id = null) : base()
        {
            _id = id;

            var criteria = new List<Expression<Func<TaskPlay, bool>>>();

            if (id.HasValue)
                criteria.Add(tp => tp.IdTask == id.Value);

            if (criteria.Any())
            {
                var parameter = Expression.Parameter(typeof(TaskPlay), "tp");
                var combinedBody = criteria
                    .Select(c => new ParameterRebinder(c.Parameters[0], parameter).Visit(c.Body))
                    .Aggregate(Expression.AndAlso);
                Criteria = Expression.Lambda<Func<TaskPlay, bool>>(combinedBody, parameter);
            }
            else
            {
                Criteria = tp => true;
            }
        }
        public override string? GetCacheKey()
        {
            var cacheKey = _id.ToString() ?? null;
            return cacheKey;
        }
    }
}
