using Microsoft.AspNetCore.SignalR;

namespace CodeBattleArena.Server.Hubs
{
    public class TaskHub : Hub
    {
        public async Task JoinTaskGroup(string taskId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Task-{taskId}");
        }

        public async Task LeaveTaskGroup(string taskId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Task-{taskId}");
        }
    }
}
