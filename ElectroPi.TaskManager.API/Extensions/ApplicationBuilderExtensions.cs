using ElectroPi.TaskManager.API.Middleware;
using ElectroPi.TaskManager.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

namespace ElectroPi.TaskManager.API.Extensions
{

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(
            this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();

        public static IApplicationBuilder UseCorrelationId(
            this IApplicationBuilder app)
            => app.UseMiddleware<CorrelationIdMiddleware>();

        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.InitialiseAsync();
        }

        public static IEndpointRouteBuilder MapHealthCheckEndpoint(
            this IEndpointRouteBuilder app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            duration = e.Value.Duration.TotalMilliseconds + "ms"
                        }),
                        totalDuration = report.TotalDuration.TotalMilliseconds + "ms"
                    };

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(result,
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
                }
            });

            return app;
        }
    }
}