using ElectroPi.TaskManager.Application.Common.Behaviors;
using ElectroPi.TaskManager.Application.Features.Tasks.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.Queries.GetTasksByProject
{

    public sealed record GetTasksByProjectQuery(
        Guid ProjectId,
        Guid RequestingUserId,
        bool BypassCache = false
    ) : IRequest<IReadOnlyList<TaskDto>>, ICacheableQuery
    {
        public string CacheKey => $"tasks:project:{ProjectId}";
        public TimeSpan CacheExpiry => TimeSpan.FromMinutes(3);
    }
}
