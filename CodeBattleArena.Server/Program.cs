using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Repositories;
using CodeBattleArena.Server.Services;
using CodeBattleArena.Server.Services.DBServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Travel_Agency.Service;
using System.Text.Json.Serialization;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Hubs;
using CodeBattleArena.Server.Services.Notifications;
using CodeBattleArena.Server.Services.Notifications.INotifications;
using CodeBattleArena.Server.Services.Judge0;
using CodeBattleArena.Server.QuestSystem;
using CodeBattleArena.Server.QuestSystem.Dispatcher;
using CodeBattleArena.Server.Untils;
using System.Threading.RateLimiting;
using StackExchange.Redis;
using CodeBattleArena.Server.Services.Cache;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Services.Cache.DecorateDBService;
using CodeBattleArena.Server.Repositories.IRepositories;
using CodeBattleArena.Server.Services.Gateways.IGateways;
using CodeBattleArena.Server.Services.Gateways;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); //ENUM Сериализация
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
    });

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHttpClient();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:55689")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleOAuth:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleOAuth:ClientSecret"];
    options.SaveTokens = true;
})
.AddCookie();

builder.Services.AddScoped<GoogleAuthService>();

builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CodeBattleArena API",
        Version = "v1",
        Description = "API for CodeBattleArena",
    });

    // Избегаем конфликтов имен
    c.CustomSchemaIds(type => type.FullName);
});

// Настройка rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("api-policy", httpContext =>
    {
        var path = httpContext.Request.Path.Value ?? "";

        // НЕ применять лимит к update-code-player
        if (path.Contains("/api/PlayerSession/update-code-player", StringComparison.OrdinalIgnoreCase))
        {
            return RateLimitPartition.GetNoLimiter("excluded");
        }

        // Применять ко всем остальным /api
        if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
        {
            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 12,
                    Window = TimeSpan.FromSeconds(3),
                    QueueLimit = 0,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                });
        }

        return RateLimitPartition.GetNoLimiter("other");
    });

    options.RejectionStatusCode = 429;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"error\":\"Oops, too many requests. Please slow down.\"}",
            cancellationToken: token);
    };
});



//------ DATABASE ------
var connectionStringBD = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options => {
    options.LogTo(Console.WriteLine);
    options.UseSqlServer(connectionStringBD);
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddIdentity<Player, IdentityRole>()
    .AddEntityFrameworkStores<AppDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<IPlayerSessionRepository, PlayerSessionRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ILangProgrammingRepository, LangProgrammingRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IQuestRepository, QuestRepository>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
builder.Services.AddScoped<IPlayerItemRepository, PlayerItemRepository>();

builder.Services.AddScoped(typeof(Lazy<>), typeof(LazyResolver<>));

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IPlayerSessionService, PlayerSessionService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ILangProgrammingService, LangProgrammingService>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IQuestService, QuestService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IAIService, AIService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.Decorate<ITaskRepository, CachedTaskRepository>();
builder.Services.Decorate<ILangProgrammingRepository, CachedLangProgrammingRepository>();
builder.Services.Decorate<IItemRepository, CachedItemRepository>();
builder.Services.Decorate<IQuestRepository, CachedQuestRepository>();

builder.Services.AddScoped<IAIGenerationGateway, AIGenerationGateway>();
builder.Services.AddHttpClient("AIApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AI:Url"]);
});

builder.Services.AddHostedService<SessionObserverService>();
builder.Services.AddHostedService<QuestObserverService>();

//------ QUEST ------
builder.Services.AddScoped<QuestHandlerContext>();
builder.Services.AddScoped<GameEventDispatcher>();

//------ SIGNALR ------
builder.Services.AddScoped<ISessionNotificationService, SessionNotificationService>();
builder.Services.AddScoped<ITaskNotificationService, TaskNotificationService>();
builder.Services.AddScoped<IPlayerNotificationService, PlayerNotificationService>();

builder.Services.AddHttpClient<Judge0Client>();

//------ REDIS ------
//var redisConfig = builder.Configuration.GetConnectionString("Redis");
//builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = Enum.GetNames(typeof(CodeBattleArena.Server.Enums.Role));

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<SessionHub>("/hubs/session");
app.MapHub<TaskHub>("/hubs/task");
app.MapHub<PlayerHub>("/hubs/player");

app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("api-policy");

app.MapFallbackToFile("/index.html");

app.Run();
