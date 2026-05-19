using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Infrastructure.Services
{

    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        private string SecretKey => _configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
        private string Issuer => _configuration["Jwt:Issuer"] ?? "ElectroPi.TaskManager";
        private string Audience => _configuration["Jwt:Audience"] ?? "ElectroPi.TaskManager.Clients";
        private int ExpiryMinutes => int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        public JwtTokenService(IConfiguration configuration)
            => _configuration = configuration;

        public string GenerateToken(ApplicationUser user)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new Claim(ClaimTypes.Name,               user.FullName),
            new Claim(ClaimTypes.Role,               user.Role.ToString()),
            new Claim("uid",                         user.Id.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: GetTokenExpiry(),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetTokenExpiry()
            => DateTime.UtcNow.AddMinutes(ExpiryMinutes);
    }
}