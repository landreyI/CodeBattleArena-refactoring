using CodeBattleArena.Application.Features.Quests.Interfaces;
using CodeBattleArena.Application.Features.Quests.Models;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.PlayerQuests;

namespace CodeBattleArena.Application.Features.Quests.Strategies
{
    public class MatchPlayedStrategy : IQuestStrategy
    {
        public TaskType SupportedTaskType => TaskType.DailyMatch;
        public bool CanHandle(GameEventType eventType) => eventType == GameEventType.MatchPlayed;

        public Task ProcessAsync(PlayerQuest playerQuest, QuestTriggerContext context, CancellationToken ct)
        {
            // Ищем параметр MatchesPerDay в твоей коллекции QuestParam
            var requiredParam = playerQuest.Quest.Params
                .FirstOrDefault(p => p.Key == TaskParamKey.MatchesPerDay);

            if (requiredParam == null || !int.TryParse(requiredParam.Value, out var requiredCount))
                return Task.CompletedTask;

            // Считаем прогресс
            var currentProgress = int.TryParse(playerQuest.ProgressValue, out var p) ? p : 0;
            currentProgress++;

            // Обновляем доменную сущность
            playerQuest.UpdateProgress(currentProgress.ToString());

            if (currentProgress >= requiredCount)
                playerQuest.Complete();

            return Task.CompletedTask;
        }
    }
}
