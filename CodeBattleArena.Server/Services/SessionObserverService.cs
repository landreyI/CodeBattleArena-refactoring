using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.DBServices.IDBServices;

namespace CodeBattleArena.Server.Services
{
    public class SessionObserverService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SessionObserverService> _logger;

        public SessionObserverService(IServiceScopeFactory scopeFactory, ILogger<SessionObserverService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var deleteTask = RunPeriodicTaskAsync(
                interval: TimeSpan.FromMinutes(30),
                action: async (scope, ct) =>
                {
                    var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
                    await sessionService.DeleteExpiredSessionsInDbAsync(DateTime.UtcNow, ct);
                },
                stoppingToken
            );

            var finishTask = RunPeriodicTaskAsync(
                interval: TimeSpan.FromMinutes(1),
                action: async (scope, ct) =>
                {
                    var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
                    await sessionService.FinishExpiredSessionsInDbAsync(DateTime.UtcNow, ct);
                },
                stoppingToken
            );

            await Task.WhenAll(deleteTask, finishTask);
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
