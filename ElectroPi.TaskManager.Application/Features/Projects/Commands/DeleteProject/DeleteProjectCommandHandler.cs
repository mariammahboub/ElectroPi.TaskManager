using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.Commands.DeleteProject
{
    public sealed class DeleteProjectCommandHandler
        : IRequestHandler<DeleteProjectCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public DeleteProjectCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Unit> Handle(
            DeleteProjectCommand command,
            CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(command.ProjectId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Project), command.ProjectId);

            if (!project.IsOwnedBy(command.RequestingUserId))
                throw new ForbiddenException("You can only delete projects that you own.");

            _unitOfWork.Projects.Delete(project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.RemoveByPrefixAsync($"projects:user:{command.RequestingUserId}", cancellationToken);
            await _cache.RemoveAsync($"projects:{command.ProjectId}", cancellationToken);

            return Unit.Value;
        }
    }
}