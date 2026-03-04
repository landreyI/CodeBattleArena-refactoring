using Ardalis.Specification;
using CodeBattleArena.Domain.PlayerQuests;

namespace CodeBattleArena.Application.Features.Quests.Specifications
{
    public class ActivePlayerQuestsWithParamsSpec : Specification<PlayerQuest>
    {
        public ActivePlayerQuestsWithParamsSpec(Guid playerId)
        {
            Query.Where(pq => pq.PlayerId == playerId && !pq.IsCompleted);

            Query.Include(pq => pq.Quest);
        }
    }
}