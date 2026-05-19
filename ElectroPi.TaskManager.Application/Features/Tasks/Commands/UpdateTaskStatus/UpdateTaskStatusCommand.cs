using ElectroPi.TaskManager.Application.Features.Tasks.DTOs;
using ElectroPi.TaskManager.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.Commands.UpdateTaskStatus
{
    public sealed record UpdateTaskStatusCommand(
        Guid TaskId,
        ProjectTaskStatus NewStatus,
        Guid RequestingUserId
    ) : IRequest<TaskDto>;
}
