using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Application.Features.Tasks.DTOs;
using ElectroPi.TaskManager.Domain.Errors;
using ElectroPi.TaskManager.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.Commands.UpdateTaskStatus
{

    public sealed class UpdateTaskStatusCommandHandler
        : IRequestHandler<UpdateTaskStatusCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public UpdateTaskStatusCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<TaskDto> Handle(
            UpdateTaskStatusCommand command,
            CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(command.TaskId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.ProjectTask), command.TaskId);

            var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Project), task.ProjectId);

            if (!project.IsOwnedBy(command.RequestingUserId))
                throw new ForbiddenException("You do not have access to this task.");

            try
            {
                task.UpdateStatus(command.NewStatus);
            }
            catch (DomainError ex)
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["Status"] = [ex.Message]
                    });
            }

            task.SetUpdated(command.RequestingUserId);
            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"tasks:project:{task.ProjectId}", cancellationToken);

            return task.Adapt<TaskDto>();
        }
    }
}