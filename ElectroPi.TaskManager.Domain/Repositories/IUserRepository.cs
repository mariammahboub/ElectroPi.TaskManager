using ElectroPi.TaskManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Repositories
{

    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {

        Task<ApplicationUser?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);


        Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken = default);
    }
}