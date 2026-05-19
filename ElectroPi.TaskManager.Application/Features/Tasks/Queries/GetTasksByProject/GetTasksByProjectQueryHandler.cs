using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Features.Tasks.DTOs;
using ElectroPi.TaskManager.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.Queries.GetTasksByProject
{

    public sealed class GetTasksByProjectQueryHandler
        : IRequestHandler<GetTasksByProjectQuery, IReadOnlyList<TaskDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTasksByProjectQueryHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<TaskDto>> Handle(
            GetTasksByProjectQuery query,
            CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(
                query.ProjectId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Project), query.ProjectId);

            if (!project.IsOwnedBy(query.RequestingUserId))
                throw new ForbiddenException("You do not have access to this project's tasks.");

            var tasks = await _unitOfWork.Tasks.GetAllByProjectAsync(
                query.ProjectId, cancellationToken);

            return tasks.Adapt<IReadOnlyList<TaskDto>>();
        }
    }
}