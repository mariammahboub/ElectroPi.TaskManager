using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.Commands.DeleteTask
{

    public sealed record DeleteTaskCommand(
        Guid TaskId,
        Guid RequestingUserId
    ) : IRequest<Unit>;
}
