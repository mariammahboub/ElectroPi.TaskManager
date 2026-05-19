using Asp.Versioning;
using ElectroPi.TaskManager.API.Extensions;
using ElectroPi.TaskManager.Application.Common.Models;
using ElectroPi.TaskManager.Application.Features.Projects.Commands.CreateProject;
using ElectroPi.TaskManager.Application.Features.Projects.Commands.DeleteProject;
using ElectroPi.TaskManager.Application.Features.Projects.Commands.UpdateProject;
using ElectroPi.TaskManager.Application.Features.Projects.DTOs;
using ElectroPi.TaskManager.Application.Features.Projects.Queries.GetAllProjects;
using ElectroPi.TaskManager.Application.Features.Projects.Queries.GetProjectById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPi.TaskManager.API.Controllers.v1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/projects")]
    [Authorize]
    [Produces("application/json")]
    public sealed class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProjectDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllProjectsQuery(OwnerId: User.GetUserId());
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetProjectByIdQuery(
                ProjectId: id,
                RequestingUserId: User.GetUserId());

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
            [FromBody] CreateProjectRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new CreateProjectCommand(
                Name: request.Name,
                Description: request.Description,
                OwnerId: User.GetUserId());

            var result = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateProjectRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateProjectCommand(
                ProjectId: id,
                Name: request.Name,
                Description: request.Description,
                RequestingUserId: User.GetUserId());

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteProjectCommand(
                ProjectId: id,
                RequestingUserId: User.GetUserId());

            await _mediator.Send(command, cancellationToken);
            return Ok(ApiResponse.DeletedResult("Project deleted successfully."));
        }
    }
}