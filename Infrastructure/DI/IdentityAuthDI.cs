using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Security;
using CodeBattleArena.Infrastructure.Identity;
using CodeBattleArena.Infrastructure.Identity.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBattleArena.Infrastructure.DI
{
    public static class IdentityAuthDI
    {
        public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration config)
        {
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

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanEditPolicy", policy =>
                    policy.Requirements.Add(new CanEditRequirement()));
                options.AddPolicy("CanModerationPolicy", policy =>
                    policy.Requirements.Add(new CanModerationRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, CanEditHandler>();
            services.AddSingleton<IAuthorizationHandler, CanModerationHandler>();

            // Сервисы аутентификации
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

            return services;
        }
    }
}
