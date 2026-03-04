using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Hubs;
using CodeBattleArena.Server.Services.Notifications.INotifications;
using Microsoft.AspNetCore.SignalR;

namespace CodeBattleArena.Server.Services.Notifications
{
    public class PlayerNotificationService : IPlayerNotificationService
    {
        private readonly IHubContext<PlayerHub> _hubContext;

        public PlayerNotificationService(IHubContext<PlayerHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyFriendRequestAsync(string idReceiver, PlayerDto sender)
        {
            await _hubContext.Clients.User(idReceiver).SendAsync("FriendRequest", sender);
        }
        public async Task NotifyInvitationSessionAsync(string idPlayer, SessionDto session)
        {
            await _hubContext.Clients.User(idPlayer).SendAsync("InvitationSession", session);
        }
    }
}
