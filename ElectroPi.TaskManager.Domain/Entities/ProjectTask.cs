using ElectroPi.TaskManager.Domain.Common;
using ElectroPi.TaskManager.Domain.Enums;
using ElectroPi.TaskManager.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Entities
{
    public sealed class ProjectTask : AuditableEntity
    {

        public string Title { get; private set; }

        public string? Description { get; private set; }

        public ProjectTaskStatus Status { get; private set; }

        public TaskPriority Priority { get; private set; }

        public DateTime? DueDate { get; private set; }

        public Guid ProjectId { get; private set; }

        public Project? Project { get; private set; }

        private ProjectTask() : base()
        {
            Title = string.Empty;
        }

        internal static ProjectTask Create(
            string title,
            string? description,
            TaskPriority priority,
            DateTime? dueDate,
            Guid projectId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.", nameof(projectId));

            return new ProjectTask
            {
                Title = title.Trim(),
                Description = description?.Trim(),
                Priority = priority,
                DueDate = dueDate?.ToUniversalTime(),
                ProjectId = projectId,
                Status = ProjectTaskStatus.Todo
            };
        }

        public void UpdateStatus(ProjectTaskStatus newStatus)
        {
            if (newStatus <= Status)
                throw new DomainError(
                    ErrorCodes.Task.InvalidStatusTransition,
                    $"Cannot transition from '{Status}' to '{newStatus}'. Status can only move forward.");

            Status = newStatus;
        }

        public void Update(
            string title,
            string? description,
            TaskPriority priority,
            DateTime? dueDate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            Title = title.Trim();
            Description = description?.Trim();
            Priority = priority;
            DueDate = dueDate?.ToUniversalTime();
        }

        public bool IsOverdue()
            => DueDate.HasValue
               && DueDate.Value < DateTime.UtcNow
               && Status != ProjectTaskStatus.Done;
    }
}