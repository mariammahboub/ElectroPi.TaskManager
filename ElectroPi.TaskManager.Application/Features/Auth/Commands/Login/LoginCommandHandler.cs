using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Application.Features.Auth.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Auth.Commands.Login
{

    public sealed class LoginCommandHandler
        : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
            => _authService = authService;

        public async Task<AuthResponseDto> Handle(
            LoginCommand command,
            CancellationToken cancellationToken)
        {
            var request = new LoginRequestDto(command.Email, command.Password);
            return await _authService.LoginAsync(request, cancellationToken);
        }
    }
}
