using ElectroPi.TaskManager.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ElectroPi.TaskManager.API.Filters
{

    public sealed class ApiResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is not null) return;

            if (context.Result is ObjectResult { Value: ApiResponse<object> }) return;

            if (context.Result is ObjectResult objectResult)
            {
                var wrapped = objectResult.StatusCode switch
                {
                    201 => ApiResponse<object>.Created(objectResult.Value!),
                    204 => ApiResponse<object>.NoContent(),
                    _ => ApiResponse<object>.Ok(objectResult.Value!)
                };

                var correlationId = context.HttpContext.Items["X-Correlation-Id"]?.ToString()
                                 ?? string.Empty;

                context.Result = new ObjectResult(wrapped.WithTraceId(correlationId))
                {
                    StatusCode = objectResult.StatusCode ?? 200
                };
            }
        }
    }
}