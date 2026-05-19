using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Exceptions
{
    public sealed class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "You do not have permission to perform this action.")
            : base(message) { }
    }
}
