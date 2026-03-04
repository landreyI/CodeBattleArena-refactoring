using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace CodeBattleArena.Server.Hubs
{
    public class SessionHub : Hub
    {
        // Хранилище наблюдателей по sessionId
        private static readonly ConcurrentDictionary<string, HashSet<string>> Observers = new();

        public async Task JoinSessionGroup(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Session-{sessionId}");
        }

        public async Task LeaveSessionGroup(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Session-{sessionId}");
        }

        public async Task JoinAsObserver(string sessionId)
        {
            var observers = Observers.GetOrAdd(sessionId, _ => new HashSet<string>());

            lock (observers)
            {
                observers.Add(Context.ConnectionId);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"Session-{sessionId}");

            await Clients.Group($"Session-{sessionId}")
                         .SendAsync("UpdateObserversCount", observers.Count);
        }

        public async Task LeaveAsObserver(string sessionId)
        {
            if (Observers.TryGetValue(sessionId, out var observers))
            {
                lock (observers)
                {
                    observers.Remove(Context.ConnectionId);
                }

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Session-{sessionId}");

                await Clients.Group($"Session-{sessionId}")
                             .SendAsync("UpdateObserversCount", observers.Count);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var kvp in Observers)
            {
                var sessionId = kvp.Key;
                var observers = kvp.Value;

                bool removed;
                lock (observers)
                {
                    removed = observers.Remove(Context.ConnectionId);
                }

                if (removed)
                {
                    await Clients.Group($"Session-{sessionId}")
                                 .SendAsync("UpdateObserversCount", observers.Count);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
