using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.QuestSystem.Interfaces;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.QuestSpec;

namespace CodeBattleArena.Server.QuestSystem.Handlers
{
    public class RequiredLeagueQuestHandler : IQuestTriggerHandler
    {
        private readonly IQuestService _questService;
        private readonly ILeagueService _leaderService;
        public RequiredLeagueQuestHandler(IQuestService questService, ILeagueService leagueService)
        {
            _questService = questService;
            _leaderService = leagueService;
        }
        public bool CanHandle(GameEventType eventType) => 
            eventType == GameEventType.Victory;

        public async Task HandleAsync(GameEventContext context, CancellationToken cancellationToken, bool commit = true)
        {
            await _questService.EnsurePlayerTaskPlayExistsForType(context.PlayerId, TaskType.WinCount, cancellationToken);

            var spec = Specification<PlayerTaskPlay>.Combine(
                new PlayerTaskPlayIncludesPlayer(),
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

                if (playerTask.IsCompleted)
                    continue;


                var requiredLeague = playerTask.TaskPlay.GetStringParam(TaskParamKey.RequiredLeague);
                if (string.IsNullOrEmpty(requiredLeague)) continue;

                var requiredIdLeague = playerTask.TaskPlay.GetIntParam(TaskParamKey.RequiredId);
                if (!requiredIdLeague.HasValue) continue;

                var reqiredLeagueDbName = await _leaderService.GetLeagueByNameAsync(requiredLeague, cancellationToken);
                var reqiredLeagueDbId = await _leaderService.GetLeagueAsync(requiredIdLeague.Value, cancellationToken);
                if(reqiredLeagueDbName != reqiredLeagueDbId)
                    continue;

                var progressDb = await _leaderService.GetLeagueByPlayerAsync(context.PlayerId, cancellationToken);
                playerTask.ProgressValue = progressDb.Name;

                if (reqiredLeagueDbId != null && reqiredLeagueDbId.MinWins <= playerTask?.Player?.Victories)
                {
                    var progress = reqiredLeagueDbId.Name;
                    playerTask.ProgressValue = progress.ToString();

                    if (requiredLeague.ToLowerInvariant() == progress.ToLowerInvariant() 
                            || requiredIdLeague == reqiredLeagueDbId.IdLeague)
                        playerTask.IsCompleted = true;
                }
            }
            await _questService.UpdatePlayerTaskPlaysInBdAsync(playersTasks, cancellationToken, commit);
        }
    }
}
