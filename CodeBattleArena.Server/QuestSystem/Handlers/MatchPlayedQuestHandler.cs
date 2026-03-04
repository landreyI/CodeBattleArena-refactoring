using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.QuestSystem.Interfaces;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.QuestSpec;

namespace CodeBattleArena.Server.QuestSystem.Handlers
{
    public class MatchPlayedQuestHandler : IQuestTriggerHandler
    {
        private readonly IQuestService _questService;
        public MatchPlayedQuestHandler(IQuestService questService)
        {
            _questService = questService;
        }
        public bool CanHandle(GameEventType eventType) => 
            eventType == GameEventType.MatchPlayed;

        public async Task HandleAsync(GameEventContext context, CancellationToken cancellationToken, bool commit = true)
        {
            await _questService.EnsurePlayerTaskPlayExistsForType(context.PlayerId, TaskType.DailyMatch, cancellationToken);

            var spec = Specification<PlayerTaskPlay>.Combine(
                new PlayerTaskPlayByTypeSpec(TaskType.DailyMatch),
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

                var requiredMatches = playerTask.TaskPlay.GetIntParam(TaskParamKey.MatchesPerDay);
                if (!requiredMatches.HasValue) continue;

                var progress = int.TryParse(playerTask.ProgressValue, out var p) ? p : 0;

                progress++;
                playerTask.ProgressValue = progress.ToString();

                if (progress >= requiredMatches)
                {
                    playerTask.IsCompleted = true;
                    playerTask.CompletedAt = DateTime.UtcNow;
                }
            }
            await _questService.UpdatePlayerTaskPlaysInBdAsync(playersTasks, cancellationToken, commit);
        }
    }
}
