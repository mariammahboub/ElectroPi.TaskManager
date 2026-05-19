using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Common
{

    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime? UpdatedAt { get; private set; }

        public Guid? CreatedById { get; private set; }

        public Guid? UpdatedById { get; private set; }

        protected AuditableEntity() : base() { }

        protected AuditableEntity(Guid id, DateTime createdAt) : base(id, createdAt) { }

        public void SetUpdated(Guid updatedById)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = updatedById;
        }


        public void SetCreatedBy(Guid createdById)
        {
            CreatedById = createdById;
        }
    }
}
