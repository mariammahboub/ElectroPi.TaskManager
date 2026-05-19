using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Exceptions
{
    public sealed class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "Authentication is required to access this resource.")
            : base(message) { }
    }

}
