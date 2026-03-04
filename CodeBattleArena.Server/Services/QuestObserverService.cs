using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.DBServices.IDBServices;

namespace CodeBattleArena.Server.Services
{
    public class QuestObserverService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<QuestObserverService> _logger;

        public QuestObserverService(IServiceScopeFactory scopeFactory, ILogger<QuestObserverService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var updateOrResetTaskProgress = RunPeriodicTaskAsync(
                interval: TimeSpan.FromMinutes(10),
                action: async (scope, ct) =>
                {
                    var questService = scope.ServiceProvider.GetRequiredService<IQuestService>();
                    await questService.UpdateOrResetTaskProgress(ct);
                },
                stoppingToken
            );

            await Task.WhenAll(updateOrResetTaskProgress);
        }

        private async Task RunPeriodicTaskAsync(
            TimeSpan interval,
            Func<IServiceScope, CancellationToken, Task> action,
            CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(interval);

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    await action(scope, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in periodic task.");
                }
            }
        }
    }
}
