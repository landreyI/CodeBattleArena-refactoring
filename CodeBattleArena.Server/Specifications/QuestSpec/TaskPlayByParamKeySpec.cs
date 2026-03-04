
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using System.Linq.Expressions;

namespace CodeBattleArena.Server.Specifications.SessionSpec
{
    public class TaskPlayByParamKeySpec : TaskPlayDefaultIncludesSpec
    {
        private readonly TaskType? _taskType;
        public TaskPlayByParamKeySpec(TaskType? taskType = null) : base()
        {
            _taskType = taskType;

            var criteria = new List<Expression<Func<TaskPlay, bool>>>();

            if (taskType.HasValue)
                criteria.Add(tp => tp.Type == taskType.Value);

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
            var cacheKey = "";
            cacheKey += _taskType != null ? "type:" + _taskType.ToString() : null;
            return cacheKey;
        }
    }
}
