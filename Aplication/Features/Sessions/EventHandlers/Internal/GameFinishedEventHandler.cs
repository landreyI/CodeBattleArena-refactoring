using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Players;
using CodeBattleArena.Domain.Sessions.Events.Internal;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.EventHandlers.Internal
{
    public class GameFinishedEventHandler : INotificationHandler<DomainEventNotification<GameFinishedInternalEvent>>
    {
        private readonly IRepository<Player> _playerRepository;

        public GameFinishedEventHandler(IRepository<Player> playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task Handle(DomainEventNotification<GameFinishedInternalEvent> notification, CancellationToken ct)
        {
            var session = notification.DomainEvent.Session;
            var playerIds = session.PlayerSessions.Select(ps => ps.PlayerId).ToList();

            var players = await _playerRepository.GetListAsync(p => playerIds.Contains(p.Id), ct: ct);

            foreach (var player in players)
            {
                bool isWinner = player.Id == session.WinnerId;

                if (isWinner)
                {
                    player.CompleteMatch(won: true, xpReward: 20, coinReward: 20);
                }
                else
                {
                    player.CompleteMatch(won: false, xpReward: 10, coinReward: 10);
                }
            }
        }
    }
}
