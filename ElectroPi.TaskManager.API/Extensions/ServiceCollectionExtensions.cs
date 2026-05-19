using Asp.Versioning;
using ElectroPi.TaskManager.API.Filters;

namespace ElectroPi.TaskManager.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddApiVersioningServices()
                .AddControllersWithFilters()
                .AddCorsPolicy(configuration)
                .AddHealthChecks(configuration);

            return services;
        }


        private static IServiceCollection AddApiVersioningServices(
            this IServiceCollection services)
        {
            services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new UrlSegmentApiVersionReader(),
                        new HeaderApiVersionReader("X-Api-Version"),
                        new QueryStringApiVersionReader("api-version"));
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            return services;
        }


        private static IServiceCollection AddControllersWithFilters(
            this IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.Filters.Add<ApiResponseFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy =
                        System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            services.AddEndpointsApiExplorer();

            return services;
        }

        private static IServiceCollection AddCorsPolicy(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var allowedOrigins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? ["http://localhost:3000", "http://localhost:4200"];

            services.AddCors(options =>
            {
                options.AddPolicy("ElectroPiCorsPolicy", policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("X-Correlation-Id", "api-supported-versions");
                });
            });

            return services;
        }
        private static IServiceCollection AddHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddHealthChecks()
                .AddSqlServer(
                    configuration.GetConnectionString("DefaultConnection")!,
                    name: "sql-server",
                    tags: ["db", "sql"])
                .AddRedis(
                    configuration.GetConnectionString("Redis") ?? "localhost:6379",
                    name: "redis",
                    tags: ["cache", "redis"]);

            return services;
        }
    }
}