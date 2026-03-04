using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Quests.Interfaces;
using CodeBattleArena.Application.Features.Quests.Models;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.PlayerQuests;
using CodeBattleArena.Domain.Players;

namespace CodeBattleArena.Application.Features.Quests.Strategies
{
    /*public class RequiredLeagueStrategy : IQuestStrategy
    {
        private readonly IRepository<Player> _playerRepository;
        public TaskType SupportedTaskType => TaskType.LeagueAdvance;
        public bool CanHandle(GameEventType eventType) => eventType == GameEventType.MatchPlayed;

        public async Task ProcessAsync(PlayerQuest playerQuest, QuestTriggerContext context, CancellationToken ct)
        {
            var requiredLeagueName = playerQuest.Quest.Params.First(p => p.Key == TaskParamKey.RequiredLeague).Value;

            // Получаем текущую лигу игрока из сервиса
            var player = await _playerRepository.GetFirstOrDefaultAsync(p => p.Id == context.PlayerId, ct:ct);

            playerQuest.UpdateProgress(player.League.Name);

            if (player.League.Name.Equals(requiredLeagueName, StringComparison.InvariantCultureIgnoreCase))
            {
                playerQuest.Complete();
            }
        }
    }*/
}
