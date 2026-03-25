using Amazon;
using Amazon.S3;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Infrastructure.AI.Services;
using CodeBattleArena.Infrastructure.AWS.S3;
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
            var awsOptions = config.GetSection("AWS");
            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsOptions["Region"])
            };

            services.AddSingleton<IAmazonS3>(sp =>
                new AmazonS3Client(awsOptions["AccessKey"], awsOptions["SecretKey"], s3Config));

            services.AddScoped<IFileStorageService, S3StorageService>();

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
