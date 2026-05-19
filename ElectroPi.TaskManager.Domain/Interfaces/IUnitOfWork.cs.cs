using ElectroPi.TaskManager.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Interfaces
{

    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IProjectRepository Projects { get; }
        ITaskRepository Tasks { get; }
        IUserRepository Users { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}