using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Common
{
    public abstract class BaseEntity 
    {
        public Guid Id { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        protected BaseEntity(Guid id, DateTime createdAt)
        {
            Id = id;
            CreatedAt = createdAt;
        }


        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return Id == other.Id;
        }

        public static bool operator ==(BaseEntity? left, BaseEntity? right)
            => left?.Equals(right) ?? right is null;

        public static bool operator !=(BaseEntity? left, BaseEntity? right)
            => !(left == right);

        public override int GetHashCode()
            => HashCode.Combine(GetType(), Id);
    }
}