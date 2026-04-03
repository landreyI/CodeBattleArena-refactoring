using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CodeBattleArena.Infrastructure.SignalR.Services
{
    public class PlayerNotificationService : IPlayerNotificationService
    {
        private readonly IHubContext<MainHub> _hubContext;

        public PlayerNotificationService(IHubContext<MainHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyFriendRequestAsync(Guid idReceiver, PlayerDto sender)
        {
            await _hubContext.Clients.User(idReceiver.ToString()).SendAsync("FriendRequest", sender);
        }
        public async Task NotifyInvitationSessionAsync(Guid idPlayer, SessionDto session)
        {
            await _hubContext.Clients.User(idPlayer.ToString()).SendAsync("InvitationSession", session);
        }
        public async Task NotifyItemEquippedAsync(Guid idPlayer, ItemDto item)
        {
            await _hubContext.Clients.User(idPlayer.ToString()).SendAsync("ItemEquipped", item);
        }
    }
}
