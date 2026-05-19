using ElectroPi.TaskManager.Application.Features.Tasks.DTOs;
using ElectroPi.TaskManager.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.Commands.CreateTask
{
    public sealed record CreateTaskCommand(
      string Title,
      string? Description,
      TaskPriority Priority,
      DateTime? DueDate,
      Guid ProjectId,
      Guid RequestingUserId
  ) : IRequest<TaskDto>;
}
