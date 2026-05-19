using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Domain.Interfaces;
using ElectroPi.TaskManager.Domain.Repositories;
using ElectroPi.TaskManager.Infrastructure.Identity;
using ElectroPi.TaskManager.Infrastructure.Persistence;
using ElectroPi.TaskManager.Infrastructure.Persistence.Interceptors;
using ElectroPi.TaskManager.Infrastructure.Persistence.Seeders;
using ElectroPi.TaskManager.Infrastructure.Repositories;
using ElectroPi.TaskManager.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;      
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Text;

namespace ElectroPi.TaskManager.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddDatabase(configuration)
                .AddIdentityServices()
                .AddJwtAuthentication(configuration)
                .AddRedisCache(configuration)
                .AddRepositories()
                .AddAppServices()
                .AddSeeders();

            return services;
        }


        private static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<AuditableEntityInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

                options
                    .UseSqlServer(                        
                        configuration.GetConnectionString("DefaultConnection"),
                        sql =>
                        {
                            sql.MigrationsAssembly(
                                typeof(ApplicationDbContext).Assembly.FullName);
                            sql.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                            sql.CommandTimeout(30);
                        })
                    .AddInterceptors(interceptor)
                    .EnableSensitiveDataLogging(
                        configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging"))
                    .EnableDetailedErrors(
                        configuration.GetValue<bool>("Logging:EnableDetailedErrors"));
            });

            return services;
        }


        private static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var secretKey = configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                                                       Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var body = """{"success":false,"message":"Authentication is required.","statusCode":401}""";
                            return context.Response.WriteAsync(body);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var body = """{"success":false,"message":"You do not have permission to perform this action.","statusCode":403}""";
                            return context.Response.WriteAsync(body);
                        }
                    };
                });

            services.AddAuthorization();

            return services;
        }


        private static IServiceCollection AddRedisCache(
       this IServiceCollection services,
       IConfiguration configuration)
        {
            var redisConnection = configuration.GetConnectionString("Redis")
                ?? "localhost:6379";
            var configOptions = ConfigurationOptions.Parse(redisConnection);
            configOptions.AbortOnConnectFail = false;         
            configOptions.ConnectRetry = 5;             
            configOptions.ReconnectRetryPolicy = new ExponentialRetry(5000); 


            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(configOptions));

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = configOptions;
                options.InstanceName = "ElectroPi:";
            });

            return services;
        }


        private static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }


        private static IServiceCollection AddAppServices(
            this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ICacheService, CacheService>();

            return services;
        }


        private static IServiceCollection AddSeeders(
            this IServiceCollection services)
        {
            services.AddScoped<RoleSeeder>();
            services.AddScoped<DatabaseSeeder>();

            return services;
        }
    }
}