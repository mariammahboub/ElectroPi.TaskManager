using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Infrastructure.Persistence.Seeders
{

    public sealed class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleSeeder _roleSeeder;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            ApplicationDbContext context,
            RoleSeeder roleSeeder,
            ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _roleSeeder = roleSeeder;
            _logger = logger;
        }

        public async Task InitialiseAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("[DatabaseSeeder] Applying pending migrations...");
                await _context.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("[DatabaseSeeder] Migrations applied.");

                _logger.LogInformation("[DatabaseSeeder] Seeding roles...");
                await _roleSeeder.SeedAsync(cancellationToken);
                _logger.LogInformation("[DatabaseSeeder] Seeding complete.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DatabaseSeeder] An error occurred during database initialisation.");
                throw;
            }
        }
    }
}