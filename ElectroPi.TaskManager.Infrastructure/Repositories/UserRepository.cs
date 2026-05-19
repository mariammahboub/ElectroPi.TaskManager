using ElectroPi.TaskManager.Domain.Entities;
using ElectroPi.TaskManager.Domain.Repositories;
using ElectroPi.TaskManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Infrastructure.Repositories
{

    public sealed class UserRepository
        : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<ApplicationUser?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Email == email.Trim().ToLowerInvariant(),
                    cancellationToken);

        public async Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken = default)
            => await DbSet.AnyAsync(
                u => u.Email == email.Trim().ToLowerInvariant(),
                cancellationToken);
    }
}