using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Features.Quests.Interfaces;
using CodeBattleArena.Application.Features.Quests.Models;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Sessions.Events.Internal;
using MediatR;

namespace CodeBattleArena.Application.Features.Quests.EventHandlers.Integration
{
    public class QuestGameFinishedHandler
        : INotificationHandler<DomainEventNotification<GameFinishedInternalEvent>>
    {
        private readonly IQuestService _questService;

        public QuestGameFinishedHandler(IQuestService questService)
        {
            _questService = questService;
        }

        public async Task Handle(DomainEventNotification<GameFinishedInternalEvent> notification, CancellationToken ct)
        {
            var session = notification.DomainEvent.Session;

            foreach (var ps in session.PlayerSessions)
            {
                // Передаем в контекст: победил ли игрок (для WinCount квестов)
                bool isWinner = ps.PlayerId == session.WinnerId;

                var context = new QuestTriggerContext(
                    ps.PlayerId,
                    GameEventType.MatchPlayed,
                    new Dictionary<string, object> {
                    { "IsWinner", isWinner },
                    { "PlayerId", ps.PlayerId }
                    }
                );

                await _questService.ProcessQuestsAsync(context, ct);
            }
        }
    }
}
