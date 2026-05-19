using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Application.Features.Tasks.DTOs;
using ElectroPi.TaskManager.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.Commands.CreateTask
{
    public sealed class CreateTaskCommandHandler
        : IRequestHandler<CreateTaskCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public CreateTaskCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<TaskDto> Handle(
            CreateTaskCommand command,
            CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.Projects.GetByIdWithTasksAsync(
                command.ProjectId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Project), command.ProjectId);

            if (!project.IsOwnedBy(command.RequestingUserId))
                throw new ForbiddenException("You can only add tasks to projects that you own.");

            var task = project.AddTask(
                command.Title,
                command.Description,
                command.Priority,
                command.DueDate);

            task.SetCreatedBy(command.RequestingUserId);

            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"tasks:project:{command.ProjectId}", cancellationToken);

            return task.Adapt<TaskDto>();
        }
    }
}