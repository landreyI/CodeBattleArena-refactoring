using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeBattleArena.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddPersistence(config);
            services.AddIdentityAuth(config);

            services.AddCaching(config);
            services.AddNotifications(config);

            services.AddMessaging(config);
            services.AddExternalClients(config);

            return services;
        }
    }
}
