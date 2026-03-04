using CodeBattleArena.Server.QuestSystem.Interfaces;

namespace CodeBattleArena.Server.QuestSystem.Dispatcher
{
    public class GameEventDispatcher
    {
        private readonly QuestHandlerContext _handlerContext;

        public GameEventDispatcher(QuestHandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }

        public async Task DispatchAsync(GameEventContext context, CancellationToken cancellationToken, bool commit = true)
        {
            foreach (var handler in _handlerContext.Handlers.Where(h => h.CanHandle(context.EventType)))
            {
                await handler.HandleAsync(context, cancellationToken, commit);
            }
        }
    }
}
