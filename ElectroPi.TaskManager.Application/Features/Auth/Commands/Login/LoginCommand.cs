using ElectroPi.TaskManager.Application.Features.Auth.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Auth.Commands.Login
{
    public sealed record LoginCommand(
      string Email,
      string Password
  ) : IRequest<AuthResponseDto>;
}
