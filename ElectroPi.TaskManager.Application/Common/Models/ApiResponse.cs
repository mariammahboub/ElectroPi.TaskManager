using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Models
{

    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }
        public IList<string>? Errors { get; init; }
        public int StatusCode { get; init; }
        public string TraceId { get; init; } = string.Empty;


        public static ApiResponse<T> Ok(T data, string message = "Request completed successfully.")
            => new()
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 200
            };

        public static ApiResponse<T> Created(T data, string message = "Resource created successfully.")
            => new()
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 201
            };

        public static ApiResponse<T> NoContent(string message = "Operation completed.")
            => new()
            {
                Success = true,
                Data = default,
                Message = message,
                StatusCode = 204
            };


        public static ApiResponse<T> NotFound(string message = "Resource not found.")
            => new()
            {
                Success = false,
                Message = message,
                StatusCode = 404
            };

        public static ApiResponse<T> Unauthorized(string message = "Authentication is required.")
            => new()
            {
                Success = false,
                Message = message,
                StatusCode = 401
            };

        public static ApiResponse<T> Forbidden(string message = "You do not have permission to perform this action.")
            => new()
            {
                Success = false,
                Message = message,
                StatusCode = 403
            };

        public static ApiResponse<T> BadRequest(string message, IList<string>? errors = null)
            => new()
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = 400
            };

        public static ApiResponse<T> Conflict(string message = "A conflict occurred with an existing resource.")
            => new()
            {
                Success = false,
                Message = message,
                StatusCode = 409
            };

        public static ApiResponse<T> InternalError(string message = "An unexpected error occurred. Please try again later.")
            => new()
            {
                Success = false,
                Message = message,
                StatusCode = 500
            };

     
        public ApiResponse<T> WithTraceId(string traceId)
            => new()
            {
                Success = this.Success,
                Message = this.Message,
                Data = this.Data,
                Errors = this.Errors,
                StatusCode = this.StatusCode,
                TraceId = traceId
            };
    }

  
    public sealed class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse SuccessResult(string message = "Operation completed successfully.")
            => new()
            {
                Success = true,
                Message = message,
                StatusCode = 200
            };

        public static ApiResponse DeletedResult(string message = "Resource deleted successfully.")
            => new()
            {
                Success = true,
                Message = message,
                StatusCode = 200
            };
    }
}