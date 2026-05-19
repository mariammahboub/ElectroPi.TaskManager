using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Application.Features.Auth.DTOs;
using ElectroPi.TaskManager.Domain.Entities;
using ElectroPi.TaskManager.Domain.Enums;
using ElectroPi.TaskManager.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Infrastructure.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(
            RegisterRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var emailExists = await _unitOfWork.Users.EmailExistsAsync(
                request.Email, cancellationToken);

            if (emailExists)
                throw new ConflictException(
                    nameof(ApplicationUser),
                    $"The email address '{request.Email}' is already registered.");

            var user = ApplicationUser.Create(
                request.FullName,
                request.Email,
                "placeholder",
                UserRole.Member);

            var hash = _passwordHasher.HashPassword(user, request.Password);
            user.UpdatePasswordHash(hash);

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken)
                ?? throw new UnauthorizedException("Invalid email address or password.");

            if (!user.IsActive)
                throw new UnauthorizedException("This account has been deactivated. Please contact support.");

            var trackedUser = await GetTrackedUserAsync(user.Id, cancellationToken)
                ?? throw new UnauthorizedException("Invalid email address or password.");

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                trackedUser, trackedUser.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedException("Invalid email address or password.");

            trackedUser.RecordLogin();
            _unitOfWork.Users.Update(trackedUser);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BuildAuthResponse(trackedUser);
        }


        private AuthResponseDto BuildAuthResponse(ApplicationUser user)
        {
            var token = _jwtTokenService.GenerateToken(user);
            var expiry = _jwtTokenService.GetTokenExpiry();

            return new AuthResponseDto(
                UserId: user.Id,
                FullName: user.FullName,
                Email: user.Email,
                Role: user.Role.ToString(),
                Token: token,
                TokenExpiry: expiry);
        }

        private async Task<ApplicationUser?> GetTrackedUserAsync(
            Guid userId,
            CancellationToken cancellationToken)
            => await _unitOfWork.Users.FirstOrDefaultAsync(
                u => u.Id == userId,
                cancellationToken);
    }
}