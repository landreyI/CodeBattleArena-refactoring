using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.PlayerQuests;
using CodeBattleArena.Domain.QuestParams;
using CodeBattleArena.Domain.QuestRewards;
using CodeBattleArena.Domain.Quests.Value_Objects;

namespace CodeBattleArena.Domain.Quests
{
    public class Quest : BaseEntity<Guid>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TaskType Type { get; private set; }
        public QuestRecurrence Recurrence { get; private set; }

        private readonly List<QuestReward> _rewards = new();
        public virtual ICollection<QuestReward> Rewards => _rewards.AsReadOnly();

        private readonly List<QuestParam> _params = new();
        public virtual ICollection<QuestParam> Params => _params.AsReadOnly();

        private readonly List<PlayerQuest> _playerQuests = new();
        public virtual ICollection<PlayerQuest> PlayerQuests => _playerQuests.AsReadOnly();

        private Quest() { } // Для EF

        private Quest(string name, string description, TaskType type, QuestRecurrence recurrence)
        {
            Name = name;
            Description = description;
            Type = type;
            Recurrence = recurrence;
        }

        public static Result<Quest> Create(string name, string description, TaskType type, QuestRecurrence recurrence)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Quest>.Failure(new Error("quest.invalid_name", "Quest name cannot be empty."));

            if (string.IsNullOrWhiteSpace(description))
                return Result<Quest>.Failure(new Error("quest.invalid_description", "Quest description cannot be empty."));

            if (recurrence.RepeatAfterDays != null && recurrence.RepeatAfterDays.Value <= 0)
                return Result<Quest>.Failure(new Error("quest.repeat_after_days", "The quest rollback number must be greater than 0 days."));

            return Result<Quest>.Success(new Quest(name, description, type, recurrence));
        }

        public bool CanBeRepeated(DateTime lastCompletionDate)
        {
            if (!Recurrence.IsRepeatable || !Recurrence.RepeatAfterDays.HasValue)
                return false;

            return DateTime.UtcNow >= lastCompletionDate.AddDays(Recurrence.RepeatAfterDays.Value);
        }

        public Result AddReward(QuestReward reward)
        {
            if (reward == null)
                return Result.Failure(new Error("quest.reward_null", "Reward cannot be null"));


            _rewards.Add(reward);
            return Result.Success();
        }
    }
}
