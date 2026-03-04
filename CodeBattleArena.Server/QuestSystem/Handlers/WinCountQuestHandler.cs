using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.QuestSystem.Interfaces;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.QuestSpec;

namespace CodeBattleArena.Server.QuestSystem.Handlers
{
    public class WinCountQuestHandler : IQuestTriggerHandler
    {
        private readonly IQuestService _questService;

        public WinCountQuestHandler(IQuestService questService)
        {
            _questService = questService;
        }

        public bool CanHandle(GameEventType eventType) =>
            eventType == GameEventType.Victory || eventType == GameEventType.Defeat;

        public async Task HandleAsync(GameEventContext context, CancellationToken cancellationToken, bool commit = true)
        {
            await _questService.EnsurePlayerTaskPlayExistsForType(context.PlayerId, TaskType.WinCount, cancellationToken);

            var spec = Specification<PlayerTaskPlay>.Combine(
                new PlayerTaskPlayByTypeSpec(TaskType.WinCount),
                new PlayerTaskPlaySpec(idPlayer: context.PlayerId)
            );

            var playersTasks = await _questService.GetListPlayerTaskPlayAsync(spec, cancellationToken);

            foreach (var playerTask in playersTasks)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (playerTask.TaskPlay?.TaskPlayParams?.Any() != true)
                    continue;

                // Авто-сброс если RepeatAfterDays прошёл + проверка на IsRepeatable
                if (playerTask.IsCompleted)
                {
                    if (QuestHelper.TryResetIfRepeatable(playerTask))
                    {
                        QuestHelper.ResetPlayerTaskPlay(playerTask, "0");
                    }
                    else
                        continue;
                }

                var requiredWins = playerTask.TaskPlay.GetIntParam(TaskParamKey.MinWins);
                if (!requiredWins.HasValue) continue;

                var resetOnLoss = playerTask.TaskPlay.GetBoolParam(TaskParamKey.ResetOnLoss);
                var progress = int.TryParse(playerTask.ProgressValue, out var p) ? p : 0;

                if (context.EventType == GameEventType.Defeat && resetOnLoss)
                {
                    playerTask.ProgressValue = "0";
                }
                else
                {
                    progress++;
                    playerTask.ProgressValue = progress.ToString();

                    if (progress >= requiredWins)
                    {
                        playerTask.IsCompleted = true;
                        playerTask.CompletedAt = DateTime.UtcNow;
                    }
                }
            }

            await _questService.UpdatePlayerTaskPlaysInBdAsync(playersTasks, cancellationToken, commit);
        }
    }
}
