using ElectroPi.TaskManager.Domain.Entities;
using ElectroPi.TaskManager.Domain.Enums;
using System;

namespace ElectroPi.TaskManager.Application.Tests.Helpers
{
    public sealed class ProjectTaskBuilder
    {
        private string _title = "Test Task";
        private string? _description = "A test task description";
        private TaskPriority _priority = TaskPriority.Medium;
        private DateTime? _dueDate = DateTime.UtcNow.AddDays(7);
        private Guid _ownerId = Guid.NewGuid();

        public ProjectTaskBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public ProjectTaskBuilder WithPriority(TaskPriority priority)
        {
            _priority = priority;
            return this;
        }

        public ProjectTaskBuilder WithDueDate(DateTime? dueDate)
        {
            _dueDate = dueDate;
            return this;
        }

        public ProjectTaskBuilder WithOwnerId(Guid ownerId)
        {
            _ownerId = ownerId;
            return this;
        }

        public (Project Project, ProjectTask Task) Build()
        {
            var project = Project.Create("Parent Project", null, _ownerId);
            var task = project.AddTask(_title, _description, _priority, _dueDate);
            return (project, task);
        }

        public static (Project Project, ProjectTask Task) Default()
            => new ProjectTaskBuilder().Build();

        public static (Project Project, ProjectTask Task) CreateWithPriority(TaskPriority priority)
            => new ProjectTaskBuilder().WithPriority(priority).Build();
    }
}