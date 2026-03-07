using CodeBattleArena.Application.Common.Behaviours;
using CodeBattleArena.Application.Common.Settings;
using CodeBattleArena.Application.Features.Quests.Interfaces;
using CodeBattleArena.Application.Features.Quests.Services;
using CodeBattleArena.Application.Features.Quests.Strategies;
using CodeBattleArena.Application.Features.Sessions.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Services;
using FluentValidation;
using MediatR;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CodeBattleArena.Application.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
        {
            ConfigureValidation();

            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
                // Notify will be called in parallel 
                cfg.NotificationPublisherType = typeof(TaskWhenAllPublisher);
            });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<ISessionAccessService, SessionAccessService>();

            services.AddScoped<IQuestService, QuestService>();

            // Регистрируем все стратегии (их будет много)
            services.AddScoped<IQuestStrategy, MatchPlayedStrategy>();
            services.AddScoped<IQuestStrategy, WinCountStrategy>();
            //services.AddScoped<IQuestStrategy, LeagueUpgradeStrategy>();

            return services;
        }

        private static void ConfigureValidation()
        {
            ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
                member?.Name.ToLower();

            ValidatorOptions.Global.ErrorCodeResolver = (validator) =>
            {
                // Просто возвращаем тип ошибки: "notempty", "email" и т.д.
                return validator.Name.Replace("Validator", "").ToLower();
            };
        }
    }
}
