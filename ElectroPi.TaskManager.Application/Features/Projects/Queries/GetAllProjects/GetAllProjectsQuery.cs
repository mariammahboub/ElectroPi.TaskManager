using ElectroPi.TaskManager.Application.Common.Behaviors;
using ElectroPi.TaskManager.Application.Features.Projects.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.Queries.GetAllProjects
{
    public sealed record GetAllProjectsQuery(
        Guid OwnerId,
        bool BypassCache = false
    ) : IRequest<IReadOnlyList<ProjectDto>>, ICacheableQuery
    {
        public string CacheKey => $"projects:user:{OwnerId}";
        public TimeSpan CacheExpiry => TimeSpan.FromMinutes(5);
    }
}