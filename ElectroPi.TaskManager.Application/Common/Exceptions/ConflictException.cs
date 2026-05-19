using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Exceptions
{
    public sealed class ConflictException : Exception
    {
        public ConflictException(string resourceName, string conflictReason)
            : base($"Conflict on '{resourceName}': {conflictReason}") { }
    }
}
