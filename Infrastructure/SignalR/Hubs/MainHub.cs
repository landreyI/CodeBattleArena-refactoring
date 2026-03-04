using CodeBattleArena.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;


namespace CodeBattleArena.Infrastructure.SignalR.Hubs
{
    public class MainHub : Hub
    {
        private readonly ICacheService _cacheService;
        private readonly string _presencePrefix = "online_user:";

        public MainHub(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        // Логика (Presence)
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                await _cacheService.IncrementAsync(_presencePrefix + userId, "connections", 1);

                await Clients.Others.SendAsync("UserOnline", userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                var remainingConnections = await _cacheService.DecrementAsync(_presencePrefix + userId, "connections", 1);

                if (remainingConnections <= 0)
                {
                    await _cacheService.RemoveAsync(_presencePrefix + userId);
                    await Clients.Others.SendAsync("UserOffline", userId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }


        public async Task SubscribeToJob(Guid jobId) => await Groups.AddToGroupAsync(Context.ConnectionId, $"Job-{jobId}");
        public async Task JoinTaskGroup(string taskId) => await Groups.AddToGroupAsync(Context.ConnectionId, $"Task-{taskId}");
        public async Task LeaveTaskGroup(string taskId) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Task-{taskId}");
    }
}
