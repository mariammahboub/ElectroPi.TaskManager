using ElectroPi.TaskManager.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Behaviors
{

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan CacheExpiry { get; }
    bool BypassCache { get; }
}

}