using ElectroPi.TaskManager.Application.Features.Projects.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.Commands.UpdateProject
{
    public sealed record UpdateProjectCommand(
        Guid ProjectId = default,  
        string Name = "",
        string? Description = null,
        Guid RequestingUserId = default  
    ) : IRequest<ProjectDto>;
}
