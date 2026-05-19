using ElectroPi.TaskManager.Application.Features.Projects.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.Commands.CreateProject
{
    public sealed record CreateProjectCommand(
      string Name,
      string? Description,
      Guid OwnerId          
  ) : IRequest<ProjectDto>;
}
