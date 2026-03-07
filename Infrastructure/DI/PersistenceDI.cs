using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Infrastructure.Identity;
using CodeBattleArena.Infrastructure.Persistence;
using CodeBattleArena.Infrastructure.Persistence.Interceptors;
using CodeBattleArena.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeBattleArena.Infrastructure.DI
{
    public static class PersistenceDI
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<DispatchDomainEventsInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));

                options.AddInterceptors(sp.GetRequiredService<DispatchDomainEventsInterceptor>());
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
