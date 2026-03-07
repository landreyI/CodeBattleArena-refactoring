using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Infrastructure.AI.Services;
using CodeBattleArena.Infrastructure.Judge0;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace CodeBattleArena.Infrastructure.DI
{
    public static class ExternalClientsDI
    {
        public static IServiceCollection AddExternalClients(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<IJudge0Client, Judge0Client>(client =>
            {
                client.BaseAddress = new Uri("https://judge0-ce.p.rapidapi.com/");
                client.DefaultRequestHeaders.Add("x-rapidapi-host", config["Judge0:ApiHost"]);
                client.DefaultRequestHeaders.Add("x-rapidapi-key", config["Judge0:ApiKey"]);
            }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(500)));

            services.AddHttpClient<IAIGenerationGateway, AIGenerationGateway>(client =>
            {
                client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
                client.Timeout = TimeSpan.FromSeconds(120); // ИИ может думать долго
            })
            // политикa повторов
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));

            services.AddScoped<IAIService, AIService>();

            return services;
        }
    }
}
