using CodeBattleArena.Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CodeBattleArena.Infrastructure.DI
{
    public static class MessagingDI
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration config)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumers(Assembly.GetExecutingAssembly());

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Очередь для создания основы задачи (Имя, Описание, Тесты)
                    cfg.ReceiveEndpoint("generate-ai-main-queue", e =>
                    {
                        e.PrefetchCount = 5; // Разрешаем обрабатывать 5 задач одновременно
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(10)));

                        e.ConfigureConsumer<GenerateAIConsumer>(context);
                    });

                    // Очередь для тяжелой генерации КОДА (ИИ + Judge0)
                    cfg.ReceiveEndpoint("generate-ai-lang-queue", e =>
                    {
                        // Это гарантирует, что мы не будем спамить в Judge0 слишком сильно.
                        e.PrefetchCount = 2;

                        // Ждем подольше перед повтором, если API нейронки "устало"
                        e.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(15)));

                        e.ConfigureConsumer<GenerateAILanguageCodeConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
