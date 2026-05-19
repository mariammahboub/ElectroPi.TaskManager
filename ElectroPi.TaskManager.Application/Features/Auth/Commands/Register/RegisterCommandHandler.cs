using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Application.Features.Auth.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Features.Auth.Commands.Register
{

    public sealed class RegisterCommandHandler
        : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAuthService authService)
            => _authService = authService;

        public async Task<AuthResponseDto> Handle(
            RegisterCommand command,
            CancellationToken cancellationToken)
        {
            var request = new RegisterRequestDto(
                command.FullName,
                command.Email,
                command.Password,
                command.ConfirmPassword);

            return await _authService.RegisterAsync(request, cancellationToken);
        }
    }
}