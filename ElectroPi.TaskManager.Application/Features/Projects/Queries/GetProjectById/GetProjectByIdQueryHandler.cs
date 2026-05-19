using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Features.Projects.DTOs;
using ElectroPi.TaskManager.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.Queries.GetProjectById
{
    public sealed class GetProjectByIdQueryHandler
        : IRequestHandler<GetProjectByIdQuery, ProjectDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProjectByIdQueryHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<ProjectDto> Handle(
            GetProjectByIdQuery query,
            CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.Projects.GetByIdWithTasksAsync(
                query.ProjectId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Project), query.ProjectId);

            if (!project.IsOwnedBy(query.RequestingUserId))
                throw new ForbiddenException("You do not have access to this project.");

            return project.Adapt<ProjectDto>();
        }
    }
}