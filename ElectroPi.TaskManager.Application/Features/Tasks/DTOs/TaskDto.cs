using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Tasks.DTOs
{
    public sealed record TaskDto(
        Guid Id,
        string Title,
        string? Description,
        string Status,
        string Priority,
        DateTime? DueDate,
        Guid ProjectId,
        DateTime CreatedAt,
        bool IsOverdue
    );
}