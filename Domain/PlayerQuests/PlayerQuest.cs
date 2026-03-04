using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;
using CodeBattleArena.Domain.Quests;

namespace CodeBattleArena.Domain.PlayerQuests
{
    public class PlayerQuest : BaseEntity<Guid>
    {
        public Guid PlayerId { get; private set; }
        public virtual Player? Player { get; private set; }

        public Guid QuestId { get; private set; }
        public virtual Quest? Quest { get; private set; }

        public bool IsCompleted { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public bool IsRewardClaimed { get; private set; }
        public string ProgressValue { get; private set; } = "0";

        private PlayerQuest() { } // Для EF

        private PlayerQuest(Guid playerId, Guid questId)
        {
            PlayerId = playerId;
            QuestId = questId;
            IsCompleted = false;
            IsRewardClaimed = false;
            ProgressValue = "0";
        }

        public static Result<PlayerQuest> Create(Guid playerId, Guid questId)
        {
            if (playerId == Guid.Empty)
                return Result<PlayerQuest>.Failure(new Error("player_quest.invalid_player", "Player ID cannot be empty."));

            if (questId == Guid.Empty)
                return Result<PlayerQuest>.Failure(new Error("player_quest.invalid_quest", "Quest ID cannot be empty."));

            return Result<PlayerQuest>.Success(new PlayerQuest(playerId, questId));
        }

        public Result UpdateProgress(string newValue)
        {
            if (IsCompleted)
                return Result.Failure(new Error("player_quest.already_completed", "Cannot update progress on a completed quest."));

            if (string.IsNullOrWhiteSpace(newValue))
                return Result.Failure(new Error("player_quest.invalid_progress", "Progress value cannot be empty."));

            ProgressValue = newValue;
            return Result.Success();
        }

        public Result Complete()
        {
            if (IsCompleted)
                return Result.Failure(new Error("player_quest.already_done", "Quest is already marked as completed."));

            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
            return Result.Success();
        }

        public Result ClaimReward()
        {
            if (!IsCompleted)
                return Result.Failure(new Error("player_quest.not_finished", "You must complete the quest before claiming the reward."));

            if (IsRewardClaimed)
                return Result.Failure(new Error("player_quest.already_claimed", "The reward for this quest has already been claimed."));

            IsRewardClaimed = true;
            return Result.Success();
        }
    }
}