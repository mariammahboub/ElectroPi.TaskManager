using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Projects.DTOs
{
    public sealed record UpdateProjectRequestDto(
      string Name,
      string? Description
  );
}
