using CodeBattleArena.Application.DI;
using CodeBattleArena.Infrastructure.DI;
using CodeBattleArena.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); //ENUM Сериализация
    });

builder.Services.AddHttpClient();

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

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Add services
builder.Services.AddControllers();

// --- Application DI ---
builder.Services.AddApplication(builder.Configuration);

// --- Infrastructure DI ---
builder.Services.AddInfrastructure(builder.Configuration);

/* Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
*/


builder.Services.AddOpenApi(options =>
{
    // Настраиваем заголовок
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "CodeBattleArena API";
        document.Info.Version = "v1";
        return Task.CompletedTask;
    });

    // Добавляем JWT авторизацию (замена того, что не работало в Swagger)
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var key = "Bearer";
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer", // именно с маленькой буквы
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Введите ваш JWT токен"
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes.Add(key, scheme);

        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = key } }] = Array.Empty<string>()
        });

        return Task.CompletedTask;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = Enum.GetNames(typeof(CodeBattleArena.Domain.Enums.Role));

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
    // Генерирует сам JSON файл по адресу /openapi/v1.json
    app.MapOpenApi();

    // интерфейс по адресу /scalar/v1
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("CodeBattleArena API")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<SessionHub>("/hubs/session");
app.MapHub<MainHub>("/hubs/main");

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
