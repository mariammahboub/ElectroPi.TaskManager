using ElectroPi.TaskManager.Domain.Common;
using ElectroPi.TaskManager.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Entities
{

    public sealed class Project : AuditableEntity
    {

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public Guid OwnerId { get; private set; }

        public ApplicationUser? Owner { get; private set; }

        private readonly List<ProjectTask> _tasks = [];

        public IReadOnlyCollection<ProjectTask> Tasks => _tasks.AsReadOnly();

        private Project() : base()
        {
            Name = string.Empty;
        }

        public static Project Create(string name, string? description, Guid ownerId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            if (ownerId == Guid.Empty)
                throw new ArgumentException("OwnerId cannot be empty.", nameof(ownerId));

            return new Project
            {
                Name = name.Trim(),
                Description = description?.Trim(),
                OwnerId = ownerId
            };
        }

        public void Update(string name, string? description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            Name = name.Trim();
            Description = description?.Trim();
        }


        public ProjectTask AddTask(
            string title,
            string? description,
            Enums.TaskPriority priority,
            DateTime? dueDate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);

            var task = ProjectTask.Create(title, description, priority, dueDate, Id);
            _tasks.Add(task);
            return task;
        }

        public void RemoveTask(Guid taskId)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId)
                ?? throw new DomainError(ErrorCodes.Task.NotFound, $"Task '{taskId}' not found in project '{Id}'.");

            _tasks.Remove(task);
        }

        public bool IsOwnedBy(Guid userId) => OwnerId == userId;
    }
}