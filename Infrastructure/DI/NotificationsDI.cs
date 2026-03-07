using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Infrastructure.SignalR.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeBattleArena.Infrastructure.DI
{
    public static class NotificationsDI
    {
        public static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration config)
        {
            var redisConnection = config.GetConnectionString("Redis") ?? "localhost:6379";

            // Настройка SignalR с Redis Backplane
            services.AddSignalR()
                .AddStackExchangeRedis(redisConnection, options => {
                    options.Configuration.ChannelPrefix = "CodeBattleArena";
                });

            // Твои доменные уведомления
            services.AddScoped<ISessionNotificationService, SessionNotificationService>();
            services.AddScoped<IPlayerNotificationService, PlayerNotificationService>();
            services.AddScoped<ITaskNotificationService, TaskNotificationService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
