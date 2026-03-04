using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Quests;

namespace CodeBattleArena.Domain.QuestParams
{
    public class QuestParam : BaseEntity<Guid>
    {
        public Guid QuestId { get; private set; }
        public virtual Quest? Quest { get; private set; }

        public TaskParamKey Key { get; private set; }
        public string Value { get; private set; }
        public bool IsPrimary { get; private set; }

        private QuestParam() { } // Для EF

        private QuestParam(Guid questId, TaskParamKey key, string value, bool isPrimary)
        {
            QuestId = questId;
            Key = key;
            Value = value;
            IsPrimary = isPrimary;
        }

        public static Result<QuestParam> Create(Guid questId, TaskParamKey key, string value, bool isPrimary = true)
        {
            if (questId == Guid.Empty)
                return Result<QuestParam>.Failure(new Error("quest_param.invalid_quest_id", "Quest ID cannot be empty."));

            if (string.IsNullOrWhiteSpace(value))
                return Result<QuestParam>.Failure(new Error("quest_param.invalid_value", "The parameter value cannot be empty."));

            return Result<QuestParam>.Success(new QuestParam(questId, key, value, isPrimary));
        }

        public Result UpdateValue(string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue))
                return Result.Failure(new Error("quest_param.update_invalid_value", "The new value cannot be empty."));

            Value = newValue;
            return Result.Success();
        }
    }
}