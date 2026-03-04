using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CodeBattleArena.Infrastructure.SignalR.Services
{
    public class SessionNotificationService : ISessionNotificationService
    {
        private readonly IHubContext<SessionHub> _hubContext;

        public SessionNotificationService(IHubContext<SessionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifySessionAddAsync(SessionDto sessionDto)
        {
            await _hubContext.Clients.All.SendAsync("SessionAdding", sessionDto);
        }
        public async Task NotifySessionDeletedGroupAsync(Guid id)
        {
            await _hubContext.Clients.Group($"Session-{id}")
                .SendAsync("SessionDeleting", id);
        }
        public async Task NotifySessionDeletedAllAsync(Guid id)
        {
            await _hubContext.Clients.All.SendAsync("SessionsListDeleting", id);
        }
        public async Task NotifySessionUpdatedGroupAsync(SessionDto sessionDto)
        {
            await _hubContext.Clients.Group($"Session-{sessionDto.Id}")
                .SendAsync("SessionUpdated", sessionDto);
        }

        public async Task NotifySessionUpdatedAllAsync(SessionDto sessionDto)
        {
            await _hubContext.Clients.All.SendAsync("SessionsListUpdated", sessionDto);
        }

        public async Task NotifySessionJoinAsync(Guid id, PlayerDto player)
        {
            await _hubContext.Clients.Group($"Session-{id}")
                .SendAsync("SessionJoin", player);
        }
        public async Task NotifySessionUnjoinAsync(Guid id, PlayerDto player)
        {
            await _hubContext.Clients.Group($"Session-{id}")
                .SendAsync("SessionLeave", player);
        }

        public async Task NotifySessionKickOutAsync(Guid id, PlayerDto player)
        {
            await _hubContext.Clients.Group($"Session-{id}")
                .SendAsync("SessionKickOut", player);
        }

        public async Task NotifyStartGameAsync(Guid id)
        {
            await _hubContext.Clients.Group($"Session-{id}")
                .SendAsync("StartGame");
        }

        public async Task NotifyFinishGameAsync(Guid id)
        {
            await _hubContext.Clients.Group($"Session-{id}")
                .SendAsync("FinishGame");
        }

        public async Task NotifyUpdateCompletedCount(Guid id, int count)
        {
            await _hubContext.Clients.Group($"Session-{id}")
               .SendAsync("UpdateCountCompleted", count);
        }

        public async Task NotifyUpdateCodePlayerAsync(Guid id, string code)
        {
            await _hubContext.Clients.Group($"Session-{id}")
                .SendAsync("UpdateCodePlayer", code);
        }
        public async Task NotifyUpdatePlayerSessionAsync(PlayerSessionDto playerSession)
        {
            await _hubContext.Clients.Group($"Session-{playerSession.SessionId}")
                .SendAsync("UpdatePlayerSession", playerSession);
        }

        /*public async Task NotifySendMessageSessionAsync(int idSession, MessageDto messageDto)
        {
            await _hubContext.Clients.Group($"Session-{idSession}")
                .SendAsync("SendMessageSession", messageDto);
        }*/
    }
}
