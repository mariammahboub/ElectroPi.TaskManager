using ElectroPi.TaskManager.Domain.Entities;
using ElectroPi.TaskManager.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Repositories
{

    public interface ITaskRepository : IBaseRepository<ProjectTask>
    {

        Task<IReadOnlyList<ProjectTask>> GetAllByProjectAsync(
            Guid projectId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ProjectTask>> GetByProjectAndStatusAsync(
            Guid projectId,
            ProjectTaskStatus status,
            CancellationToken cancellationToken = default);


        Task<IReadOnlyList<ProjectTask>> GetOverdueByProjectAsync(
            Guid projectId,
            CancellationToken cancellationToken = default);
    }
}