using CodeBattleArena.Application.Common.Behaviours;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Settings;
using CodeBattleArena.Application.Features.AI.Services;
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
using Polly;
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

            services.AddHttpClient<IAIGenerationGateway, AIGenerationGateway>(client =>
            {
                // Базовый адрес Google AI
                client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
                client.Timeout = TimeSpan.FromSeconds(120); // ИИ может думать долго
            })
            // Добавляем политику повторов на случай сетевых сбоев
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));

            services.AddScoped<IAIService, AIService>();

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
