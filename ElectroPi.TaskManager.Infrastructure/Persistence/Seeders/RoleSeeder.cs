using ElectroPi.TaskManager.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Infrastructure.Persistence.Seeders
{

    public sealed class RoleSeeder
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<RoleSeeder> _logger;

        public RoleSeeder(
            RoleManager<IdentityRole<Guid>> roleManager,
            ILogger<RoleSeeder> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var roles = Enum.GetNames<UserRole>();

            foreach (var roleName in roles)
            {
                if (await _roleManager.RoleExistsAsync(roleName)) continue;

                var role = new IdentityRole<Guid>(roleName) { Id = Guid.NewGuid() };
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                    _logger.LogInformation("[Seeder] Role '{Role}' created.", roleName);
                else
                    _logger.LogError(
                        "[Seeder] Failed to create role '{Role}': {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}