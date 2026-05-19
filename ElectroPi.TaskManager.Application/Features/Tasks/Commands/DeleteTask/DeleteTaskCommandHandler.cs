using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.Commands.DeleteTask
{
    public sealed class DeleteTaskCommandHandler
        : IRequestHandler<DeleteTaskCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public DeleteTaskCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Unit> Handle(
            DeleteTaskCommand command,
            CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(command.TaskId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.ProjectTask), command.TaskId);

            var project = await _unitOfWork.Projects.GetByIdWithTasksAsync(
                task.ProjectId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Project), task.ProjectId);

            if (!project.IsOwnedBy(command.RequestingUserId))
                throw new ForbiddenException("You can only delete tasks from projects that you own.");

            project.RemoveTask(command.TaskId);

            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"tasks:project:{task.ProjectId}", cancellationToken);

            return Unit.Value;
        }
    }
}