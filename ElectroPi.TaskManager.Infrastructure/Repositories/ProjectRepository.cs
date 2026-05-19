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

    public sealed class ProjectRepository
        : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Project>> GetAllByOwnerAsync(
            Guid ownerId,
            CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .Where(p => p.OwnerId == ownerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);


        public async Task<Project?> GetByIdWithTasksAsync(
            Guid projectId,
            CancellationToken cancellationToken = default)
            => await DbSet
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

        public async Task<bool> NameExistsForOwnerAsync(
            string name,
            Guid ownerId,
            CancellationToken cancellationToken = default)
            => await DbSet.AnyAsync(
                p => p.OwnerId == ownerId
                  && p.Name.ToLower() == name.ToLower(),
                cancellationToken);
    }
}