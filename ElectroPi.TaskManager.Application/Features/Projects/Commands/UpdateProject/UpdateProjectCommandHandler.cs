using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Application.Features.Projects.DTOs;
using ElectroPi.TaskManager.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.Commands.UpdateProject
{
    public sealed class UpdateProjectCommandHandler
        : IRequestHandler<UpdateProjectCommand, ProjectDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public UpdateProjectCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<ProjectDto> Handle(
            UpdateProjectCommand command,
            CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(command.ProjectId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Project), command.ProjectId);

            if (!project.IsOwnedBy(command.RequestingUserId))
                throw new ForbiddenException("You can only update projects that you own.");

            project.Update(command.Name, command.Description);
            project.SetUpdated(command.RequestingUserId);

            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.RemoveByPrefixAsync($"projects:user:{command.RequestingUserId}", cancellationToken);
            await _cache.RemoveAsync($"projects:{command.ProjectId}", cancellationToken);

            return project.Adapt<ProjectDto>();
        }
    }
}