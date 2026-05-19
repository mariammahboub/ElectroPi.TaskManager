using ElectroPi.TaskManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Interfaces
{

    public interface IJwtTokenService
    {

        string GenerateToken(ApplicationUser user);

        DateTime GetTokenExpiry();
    }
}