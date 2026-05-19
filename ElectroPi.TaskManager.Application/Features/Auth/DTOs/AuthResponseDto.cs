using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Auth.DTOs
{
    public sealed record AuthResponseDto(
        Guid UserId,
        string FullName,
        string Email,
        string Role,
        string Token,
        DateTime TokenExpiry
    );
}
