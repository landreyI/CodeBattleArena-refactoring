using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace CodeBattleArena.Server.Specifications.QuestSpec
{
    public class PlayerTaskPlaySpec : Specification<PlayerTaskPlay>
    {

        /// <param name="checkRepeatable">
        /// If true - filters tasks that can be repeated: 
        /// the task is completed, the reward has been received, and enough days have passed since the completion.
        /// </param>
        public PlayerTaskPlaySpec
            (int? idTaskPlay = null, string? idPlayer = null, bool? isCompleted = null, 
            bool? isGet = null, bool? checkRepeatable = null)
        {
            AddInclude(s => s.TaskPlay);

            var criteria = new List<Expression<Func<PlayerTaskPlay, bool>>>();

            if (idTaskPlay.HasValue)
                criteria.Add(pt => pt.TaskPlayId == idTaskPlay.Value);

            if (!string.IsNullOrEmpty(idPlayer))
                criteria.Add(pt => pt.PlayerId == idPlayer);

            if (checkRepeatable == true)
            {
                var now = DateTime.UtcNow;

                criteria.Add(pt =>
                    pt.IsGet && pt.IsCompleted &&
                    pt.CompletedAt.HasValue &&
                    pt.TaskPlay.RepeatAfterDays.HasValue &&
                    EF.Functions.DateDiffDay(pt.CompletedAt.Value, now) >= pt.TaskPlay.RepeatAfterDays.Value
                );
            }
            else
            {
                if (isCompleted.HasValue)
                    criteria.Add(pt => pt.IsCompleted == isCompleted.Value);

                if (isGet.HasValue)
                    criteria.Add(pt => pt.IsGet == isGet.Value);
            }

            if (criteria.Any())
            {
                var parameter = Expression.Parameter(typeof(PlayerTaskPlay), "pt");
                var combinedBody = criteria
                    .Select(c => new ParameterRebinder(c.Parameters[0], parameter).Visit(c.Body))
                    .Aggregate(Expression.AndAlso);
                Criteria = Expression.Lambda<Func<PlayerTaskPlay, bool>>(combinedBody, parameter);
            }
            else
            {
                Criteria = pt => true;
            }
        }
    }
}
