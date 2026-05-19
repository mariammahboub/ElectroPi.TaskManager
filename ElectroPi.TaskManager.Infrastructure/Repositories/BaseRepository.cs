using ElectroPi.TaskManager.Domain.Common;
using ElectroPi.TaskManager.Domain.Repositories;
using ElectroPi.TaskManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Infrastructure.Repositories
{

    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        protected BaseRepository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);

        public virtual async Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);

        public virtual async Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            => await DbSet.AnyAsync(predicate, cancellationToken);


        public virtual async Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
            => await DbSet.AddAsync(entity, cancellationToken);

        public virtual void Update(TEntity entity)
            => DbSet.Update(entity);

        public virtual void Delete(TEntity entity)
            => DbSet.Remove(entity);
    }
}