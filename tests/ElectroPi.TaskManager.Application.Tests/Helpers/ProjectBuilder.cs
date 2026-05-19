using ElectroPi.TaskManager.Domain.Entities;
using System;

namespace ElectroPi.TaskManager.Application.Tests.Helpers
{
    public sealed class ProjectBuilder
    {
        private string _name = "Test Project";
        private string? _description = "A test project description";
        private Guid _ownerId = Guid.NewGuid();

        public ProjectBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ProjectBuilder WithDescription(string? description)
        {
            _description = description;
            return this;
        }

        public ProjectBuilder WithOwnerId(Guid ownerId)
        {
            _ownerId = ownerId;
            return this;
        }

        public Project Build()
            => Project.Create(_name, _description, _ownerId);

        public static Project Default() => new ProjectBuilder().Build();

        public static Project OwnedBy(Guid ownerId) => new ProjectBuilder().WithOwnerId(ownerId).Build();

        // تم تغيير الاسم هنا إلى CreateWithName
        public static Project CreateWithName(string name) => new ProjectBuilder().WithName(name).Build();
    }
}