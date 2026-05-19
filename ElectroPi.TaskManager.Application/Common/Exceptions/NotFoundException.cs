using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Exceptions
{

    public sealed class NotFoundException : Exception
    {
        public string ResourceName { get; }
        public object ResourceKey { get; }

        public NotFoundException(string resourceName, object resourceKey)
            : base($"Resource '{resourceName}' with key '{resourceKey}' was not found.")
        {
            ResourceName = resourceName;
            ResourceKey = resourceKey;
        }
    }
}
