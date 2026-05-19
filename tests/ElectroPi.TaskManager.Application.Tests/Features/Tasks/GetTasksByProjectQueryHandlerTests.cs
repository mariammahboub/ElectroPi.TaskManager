using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Common.Mappings;      // ← correct namespace
using ElectroPi.TaskManager.Application.Features.Tasks.Queries.GetTasksByProject;
using ElectroPi.TaskManager.Application.Tests.Common.Fixtures;
using ElectroPi.TaskManager.Application.Tests.Helpers;
using ElectroPi.TaskManager.Domain.Entities;
using FluentAssertions;
using Mapster;
using Moq;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Tasks
{
    public sealed class GetTasksByProjectQueryHandlerTests
    {
        private readonly MediatorFixture _fixture = new();
        private readonly GetTasksByProjectQueryHandler _handler;

        public GetTasksByProjectQueryHandlerTests()
        {

            TypeAdapterConfig.GlobalSettings.Scan(
                typeof(TaskMappingConfig).Assembly);   

            _handler = new GetTasksByProjectQueryHandler(_fixture.UnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidQuery_ShouldReturnTasks()
        {
            var ownerId = Guid.NewGuid();
            var (project, task) = new ProjectTaskBuilder().WithOwnerId(ownerId).Build();

            var query = new GetTasksByProjectQuery(
                ProjectId: project.Id,
                RequestingUserId: ownerId);

            _fixture.ProjectRepository
                .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            _fixture.TaskRepository
                .Setup(r => r.GetAllByProjectAsync(project.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProjectTask> { task }.AsReadOnly());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Title.Should().Be(task.Title);
            result.First().ProjectId.Should().Be(project.Id);
        }

        [Fact]
        public async Task Handle_WithNonExistentProject_ShouldThrowNotFoundException()
        {
            var query = new GetTasksByProjectQuery(Guid.NewGuid(), Guid.NewGuid());

            _fixture.ProjectRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Project?)null);

            var act = async () => await _handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotOwnProject_ShouldThrowForbiddenException()
        {
            var project = ProjectBuilder.OwnedBy(Guid.NewGuid());
            var query = new GetTasksByProjectQuery(project.Id, Guid.NewGuid());

            _fixture.ProjectRepository
                .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            var act = async () => await _handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_WithNoTasks_ShouldReturnEmptyList()
        {
            var ownerId = Guid.NewGuid();
            var project = ProjectBuilder.OwnedBy(ownerId);
            var query = new GetTasksByProjectQuery(project.Id, ownerId);

            _fixture.ProjectRepository
                .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            _fixture.TaskRepository
                .Setup(r => r.GetAllByProjectAsync(project.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProjectTask>().AsReadOnly());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}