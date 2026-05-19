using ElectroPi.TaskManager.Application.Common.Behaviors;
using ElectroPi.TaskManager.Application.Features.Projects.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.Queries.GetProjectById
{
    public sealed record GetProjectByIdQuery(
     Guid ProjectId,
     Guid RequestingUserId,
     bool BypassCache = false
 ) : IRequest<ProjectDto>, ICacheableQuery
    {
        public string CacheKey => $"projects:{ProjectId}";
        public TimeSpan CacheExpiry => TimeSpan.FromMinutes(5);
    }
}
