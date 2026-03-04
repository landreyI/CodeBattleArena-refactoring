using CodeBattleArena.Application.Features.Quests.Interfaces;
using CodeBattleArena.Application.Features.Quests.Models;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.PlayerQuests;

namespace CodeBattleArena.Application.Features.Quests.Strategies
{
    public class WinCountStrategy : IQuestStrategy
    {
        public TaskType SupportedTaskType => TaskType.WinCount;
        public bool CanHandle(GameEventType eventType) => eventType == GameEventType.MatchPlayed;

        public Task ProcessAsync(PlayerQuest playerQuest, QuestTriggerContext context, CancellationToken ct)
        {
            bool isWinner = context.Metadata.ContainsKey("IsWinner") && (bool)context.Metadata["IsWinner"];

            // Читаем параметры: сколько побед нужно и сбрасывать ли при проигрыше
            var minWins = int.Parse(playerQuest.Quest.Params.First(p => p.Key == TaskParamKey.MinWins).Value);
            var resetOnLoss = playerQuest.Quest.Params.Any(p => p.Key == TaskParamKey.ResetOnLoss && p.Value == "true");

            var progress = int.TryParse(playerQuest.ProgressValue, out var p) ? p : 0;

            if (!isWinner && resetOnLoss)
            {
                playerQuest.UpdateProgress("0");
            }
            else if (isWinner)
            {
                progress++;
                playerQuest.UpdateProgress(progress.ToString());

                if (progress >= minWins)
                    playerQuest.Complete();
            }

            return Task.CompletedTask;
        }
    }
}
