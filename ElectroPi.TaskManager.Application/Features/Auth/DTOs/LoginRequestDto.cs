using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Auth.DTOs
{
    public sealed record LoginRequestDto(
       string Email,
       string Password
   );
}
