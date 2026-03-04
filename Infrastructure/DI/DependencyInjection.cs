
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Infrastructure.Identity;
using CodeBattleArena.Infrastructure.Judge0;
using CodeBattleArena.Infrastructure.Messaging.Consumers;
using CodeBattleArena.Infrastructure.Persistence;
using CodeBattleArena.Infrastructure.Persistence.Interceptors;
using CodeBattleArena.Infrastructure.Persistence.Redis;
using CodeBattleArena.Infrastructure.Persistence.Repositories;
using CodeBattleArena.Infrastructure.SignalR.Services;
using CodeBattleArena.Infrastructure.SignalR.Services.Notifications;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System.Reflection;
using System.Text;

namespace CodeBattleArena.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
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

            // Настройка JWT Аутентификации
            var jwtSettings = config.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is missing");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });



            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            // Сервисы аутентификации
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

            services.AddSignalR();
            services.AddScoped<ISessionNotificationService, SessionNotificationService>();
            services.AddScoped<IPlayerNotificationService, PlayerNotificationService>();
            services.AddScoped<ITaskNotificationService, TaskNotificationService>();

            services.AddScoped<ICacheService, RedisCacheService>();

            services.AddHttpClient<IJudge0Client, Judge0Client>(client =>
            {
                client.BaseAddress = new Uri("https://judge0-ce.p.rapidapi.com/");
                client.DefaultRequestHeaders.Add("x-rapidapi-host", config["Judge0:ApiHost"]);
                client.DefaultRequestHeaders.Add("x-rapidapi-key", config["Judge0:ApiKey"]);
            }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(500)));



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
