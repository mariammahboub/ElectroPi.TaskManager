using ElectroPi.TaskManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Repositories
{

    public interface IProjectRepository : IBaseRepository<Project>
    {

        Task<IReadOnlyList<Project>> GetAllByOwnerAsync(
            Guid ownerId,
            CancellationToken cancellationToken = default);

        Task<Project?> GetByIdWithTasksAsync(
            Guid projectId,
            CancellationToken cancellationToken = default);


        Task<bool> NameExistsForOwnerAsync(
            string name,
            Guid ownerId,
            CancellationToken cancellationToken = default);
    }
}