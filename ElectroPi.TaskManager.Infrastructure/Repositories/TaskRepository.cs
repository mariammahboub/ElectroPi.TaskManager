using ElectroPi.TaskManager.Domain.Entities;
using ElectroPi.TaskManager.Domain.Enums;
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

    public sealed class TaskRepository
        : BaseRepository<ProjectTask>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<ProjectTask>> GetAllByProjectAsync(
            Guid projectId,
            CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<ProjectTask>> GetByProjectAndStatusAsync(
            Guid projectId,
            ProjectTaskStatus status,
            CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId && t.Status == status)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<ProjectTask>> GetOverdueByProjectAsync(
            Guid projectId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            return await DbSet
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId
                         && t.DueDate.HasValue
                         && t.DueDate.Value < now
                         && t.Status != ProjectTaskStatus.Done)
                .OrderBy(t => t.DueDate)
                .ToListAsync(cancellationToken);
        }
    }
}