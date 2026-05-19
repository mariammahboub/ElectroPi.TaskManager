using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ElectroPi.TaskManager.API.Extensions
{

    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerWithVersioning(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Enter your JWT token below.\n\n" +
                        "Example: **eyJhbGciOiJIUzI1NiIs...**"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
                });

                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;

                    var versions = methodInfo.DeclaringType?
                        .GetCustomAttributes(true)
                        .OfType<Asp.Versioning.ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions)
                        .ToList();

                    if (versions == null || versions.Count == 0) return true;

                    return versions.Any(v => $"v{v.MajorVersion}" == docName);
                });

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ElectroPi Task Manager API",
                    Version = "v1",
                    Description =
                        "Project & Task Management API built with Clean Architecture, " +
                        "CQRS, MediatR, and JWT Authentication.\n\n" +
                        "**Company:** ElectroPi\n\n" +
                        "**Stack:** .NET 9 · EF Core · SQL Server · Redis",
                    Contact = new OpenApiContact
                    {
                        Name = "ElectroPi Backend Team",
                        Email = "backend@electropi.com"
                    }
                });

                foreach (var xmlFile in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
                    options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerWithVersioning(
            this IApplicationBuilder app)
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api-docs/v1/swagger.json", "ElectroPi Task Manager v1");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "ElectroPi Task Manager — API Docs";
                options.DisplayRequestDuration();
                options.EnableFilter();
                options.EnableDeepLinking();
            });

            return app;
        }
    }
}