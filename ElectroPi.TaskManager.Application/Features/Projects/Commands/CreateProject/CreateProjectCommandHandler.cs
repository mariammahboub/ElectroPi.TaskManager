using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Application.Features.Projects.DTOs;
using ElectroPi.TaskManager.Domain.Entities;
using ElectroPi.TaskManager.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.Commands.CreateProject
{

    public sealed class CreateProjectCommandHandler
        : IRequestHandler<CreateProjectCommand, ProjectDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public CreateProjectCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<ProjectDto> Handle(
            CreateProjectCommand command,
            CancellationToken cancellationToken)
        {
            var nameExists = await _unitOfWork.Projects.NameExistsForOwnerAsync(
                command.Name, command.OwnerId, cancellationToken);

            if (nameExists)
                throw new ConflictException(nameof(Project), $"A project named '{command.Name}' already exists.");

            var project = Project.Create(command.Name, command.Description, command.OwnerId);
            project.SetCreatedBy(command.OwnerId);

            await _unitOfWork.Projects.AddAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.RemoveByPrefixAsync($"projects:user:{command.OwnerId}", cancellationToken);

            return project.Adapt<ProjectDto>();
        }
    }
}