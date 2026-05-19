using ElectroPi.TaskManager.Application.Features.Projects.DTOs;
using ElectroPi.TaskManager.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.Queries.GetAllProjects
{
    public sealed class GetAllProjectsQueryHandler
      : IRequestHandler<GetAllProjectsQuery, IReadOnlyList<ProjectDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllProjectsQueryHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<ProjectDto>> Handle(
            GetAllProjectsQuery query,
            CancellationToken cancellationToken)
        {
            var projects = await _unitOfWork.Projects.GetAllByOwnerAsync(
                query.OwnerId, cancellationToken);

            return projects.Adapt<IReadOnlyList<ProjectDto>>();
        }
    }
}
