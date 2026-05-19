using ElectroPi.TaskManager.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.DTOs
{

    public sealed record CreateTaskRequestDto(
        string Title,
        string? Description,
        TaskPriority Priority,
        DateTime? DueDate
    );
}