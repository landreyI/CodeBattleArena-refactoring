using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using System.Linq.Expressions;

namespace CodeBattleArena.Server.Specifications.ItemSpec
{
    public class PlayerItemSpec : PlayerItemDefaultIncludesSpec
    {
        public PlayerItemSpec(int? idItem = null, string? idPlayer = null, TypeItem? typeItem = null) : base()
        {
            var criteria = new List<Expression<Func<PlayerItem, bool>>>();

            if (idItem != null)
                criteria.Add(pi => pi.IdItem == idItem);

            if (!string.IsNullOrEmpty(idPlayer))
                criteria.Add(pt => pt.IdPlayer == idPlayer);

            if (typeItem.HasValue)
                criteria.Add(pi => pi.Item.Type == typeItem);

            if (criteria.Any())
            {
                var parameter = Expression.Parameter(typeof(PlayerItem), "pi");
                var combinedBody = criteria
                    .Select(c => new ParameterRebinder(c.Parameters[0], parameter).Visit(c.Body))
                    .Aggregate(Expression.AndAlso);
                Criteria = Expression.Lambda<Func<PlayerItem, bool>>(combinedBody, parameter);
            }
            else
            {
                Criteria = tp => true;
            }
        }
    }
}
