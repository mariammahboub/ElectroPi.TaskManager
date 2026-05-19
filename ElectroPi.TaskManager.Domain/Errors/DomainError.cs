using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Errors
{

    public sealed class DomainError : Exception
    {
        public string Code { get; }

        public DomainError(string code, string message)
            : base(message)
        {
            Code = code;
        }

        public DomainError(string code, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }

        public override string ToString()
            => $"[DomainError] Code: {Code} | Message: {Message}";
    }
}