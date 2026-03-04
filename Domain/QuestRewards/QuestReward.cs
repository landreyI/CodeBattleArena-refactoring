using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Quests;
using CodeBattleArena.Domain.Rewards;

namespace CodeBattleArena.Domain.QuestRewards
{
    public class QuestReward : BaseEntity<Guid>
    {
        public Guid QuestId { get; private set; }
        public virtual Quest? Quest { get; private set; }

        public Guid RewardId { get; private set; }
        public virtual Reward? Reward { get; private set; }

        private QuestReward() { } // Для EF

        private QuestReward(Guid questId, Guid rewardId)
        {
            QuestId = questId;
            RewardId = rewardId;
        }

        public static Result<QuestReward> Create(Guid questId, Guid rewardId)
        {
            if (questId == Guid.Empty)
                return Result<QuestReward>.Failure(new Error("quest_reward.quest_id", "Quest ID cannot be empty"));
            if (rewardId == Guid.Empty)
                return Result<QuestReward>.Failure(new Error("quest_reward.reward_id", "Reward ID cannot be empty"));

            return Result<QuestReward>.Success(new QuestReward(questId, rewardId));
        }
    }
}
