using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Quests.Interfaces;
using CodeBattleArena.Application.Features.Quests.Models;
using CodeBattleArena.Application.Features.Quests.Specifications;
using CodeBattleArena.Domain.PlayerQuests;

namespace CodeBattleArena.Application.Features.Quests.Services
{
    public class QuestService : IQuestService
    {
        private readonly IRepository<PlayerQuest> _playerQuestRepository;
        private readonly IEnumerable<IQuestStrategy> _strategies;

        public QuestService(
            IRepository<PlayerQuest> playerQuestRepository,
            IEnumerable<IQuestStrategy> strategies)
        {
            _playerQuestRepository = playerQuestRepository;
            _strategies = strategies;
        }

        public async Task ProcessQuestsAsync(QuestTriggerContext context, CancellationToken ct)
        {
            var spec = new ActivePlayerQuestsWithParamsSpec(context.PlayerId);
            var activeQuests = await _playerQuestRepository.GetListBySpecAsync(spec, ct);

            if (!activeQuests.Any()) return;

            foreach (var playerQuest in activeQuests)
            {
                // 2. Ищем стратегию, которая:
                // а) Поддерживает тип этого квеста (например, DailyMatch)
                // б) Умеет обрабатывать пришедшее событие (например, MatchPlayed)
                var strategy = _strategies.FirstOrDefault(s =>
                    s.SupportedTaskType == playerQuest.Quest.Type &&
                    s.CanHandle(context.EventType));

                if (strategy != null)
                {
                    // 3. Передаем управление стратегии
                    // Она обновит состояние PlayerQuest в памяти
                    await strategy.ProcessAsync(playerQuest, context, ct);
                }
            }

            // ВАЖНО: Мы здесь НЕ вызываем _unitOfWork.CommitAsync().
            // Если этот метод вызван из FinishGameHandler, то общий Commit в конце 
            // сохранит изменения и в Сессии, и во всех обновленных PlayerQuests.
        }
    }
}
