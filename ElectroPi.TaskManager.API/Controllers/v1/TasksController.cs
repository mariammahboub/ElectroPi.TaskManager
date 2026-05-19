using Asp.Versioning;
using ElectroPi.TaskManager.API.Extensions;
using ElectroPi.TaskManager.Application.Common.Models;
using ElectroPi.TaskManager.Application.Features.Tasks.Commands.CreateTask;
using ElectroPi.TaskManager.Application.Features.Tasks.Commands.DeleteTask;
using ElectroPi.TaskManager.Application.Features.Tasks.Commands.UpdateTaskStatus;
using ElectroPi.TaskManager.Application.Features.Tasks.DTOs;
using ElectroPi.TaskManager.Application.Features.Tasks.Queries.GetTasksByProject;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPi.TaskManager.API.Controllers.v1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/projects/{projectId:guid}/tasks")]
    [Authorize]
    [Produces("application/json")]
    public sealed class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TaskDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByProject(
            [FromRoute] Guid projectId,
            CancellationToken cancellationToken)
        {
            var query = new GetTasksByProjectQuery(
                ProjectId: projectId,
                RequestingUserId: User.GetUserId());

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(
            [FromRoute] Guid projectId,
            [FromBody] CreateTaskRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new CreateTaskCommand(
                Title: request.Title,
                Description: request.Description,
                Priority: request.Priority,
                DueDate: request.DueDate,
                ProjectId: projectId,
                RequestingUserId: User.GetUserId());

            var result = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPatch("{taskId:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(
            [FromRoute] Guid projectId,
            [FromRoute] Guid taskId,
            [FromBody] UpdateTaskStatusRequestDto request,
            CancellationToken cancellationToken)
        {
            _ = projectId;

            var command = new UpdateTaskStatusCommand(
                TaskId: taskId,
                NewStatus: request.NewStatus,
                RequestingUserId: User.GetUserId());

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }


        [HttpDelete("{taskId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid projectId,
            [FromRoute] Guid taskId,
            CancellationToken cancellationToken)
        {
            _ = projectId;

            var command = new DeleteTaskCommand(
                TaskId: taskId,
                RequestingUserId: User.GetUserId());

            await _mediator.Send(command, cancellationToken);
            return Ok(ApiResponse.DeletedResult("Task deleted successfully."));
        }
    }
}