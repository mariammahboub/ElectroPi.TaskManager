using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.DTOs
{
    public sealed record ProjectDto(
        Guid Id,
        string Name,
        string? Description,
        DateTime CreatedAt,
        int TaskCount
    );
}
