using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Common.Models;
using ElectroPi.TaskManager.Domain.Errors;
using System.Net;
using System.Text.Json;

namespace ElectroPi.TaskManager.API.Middleware
{

    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = context.Items["X-Correlation-Id"]?.ToString()
                             ?? Guid.NewGuid().ToString();

            var (statusCode, response) = exception switch
            {
                ValidationException ve => (
                    HttpStatusCode.BadRequest,
                    ApiResponse<object>.BadRequest(
                        "One or more validation failures have occurred.",
                        ve.Errors.SelectMany(e => e.Value).ToList())
                        .WithTraceId(correlationId)),

                NotFoundException nfe => (
                    HttpStatusCode.NotFound,
                    ApiResponse<object>.NotFound(nfe.Message)
                        .WithTraceId(correlationId)),

                UnauthorizedException ue => (
                    HttpStatusCode.Unauthorized,
                    ApiResponse<object>.Unauthorized(ue.Message)
                        .WithTraceId(correlationId)),

                ForbiddenException fe => (
                    HttpStatusCode.Forbidden,
                    ApiResponse<object>.Forbidden(fe.Message)
                        .WithTraceId(correlationId)),

                ConflictException ce => (
                    HttpStatusCode.Conflict,
                    ApiResponse<object>.Conflict(ce.Message)
                        .WithTraceId(correlationId)),

                DomainError de => (
                    HttpStatusCode.BadRequest,
                    ApiResponse<object>.BadRequest($"[{de.Code}] {de.Message}")
                        .WithTraceId(correlationId)),

                OperationCanceledException => (
                    HttpStatusCode.BadRequest,
                    ApiResponse<object>.BadRequest("The request was cancelled.")
                        .WithTraceId(correlationId)),

                _ => (
                    HttpStatusCode.InternalServerError,
                    ApiResponse<object>.InternalError()
                        .WithTraceId(correlationId))
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(
                    exception,
                    "[GlobalException] Unhandled exception on {Method} {Path} | CorrelationId: {CorrelationId}",
                    context.Request.Method, context.Request.Path, correlationId);
            else
                _logger.LogWarning(
                    "[GlobalException] Handled {ExceptionType} on {Method} {Path} | {Message} | CorrelationId: {CorrelationId}",
                    exception.GetType().Name,
                    context.Request.Method, context.Request.Path,
                    exception.Message, correlationId);

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, JsonOptions));
        }
    }
}