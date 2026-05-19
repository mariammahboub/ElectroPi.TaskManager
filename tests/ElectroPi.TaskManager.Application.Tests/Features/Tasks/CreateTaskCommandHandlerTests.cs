using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Features.Tasks.Commands.CreateTask;
using ElectroPi.TaskManager.Application.Tests.Common.Fixtures;
using ElectroPi.TaskManager.Application.Tests.Helpers;
using ElectroPi.TaskManager.Domain.Enums;
using FluentAssertions;
using Mapster;
using Moq;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Tasks;

public sealed class CreateTaskCommandHandlerTests
{
    private readonly MediatorFixture _fixture = new();
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(CreateTaskCommand).Assembly);

        _handler = new CreateTaskCommandHandler(
            _fixture.UnitOfWork.Object,
            _fixture.CacheService.Object);
    }
    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnTaskDto()
    {
        var ownerId = Guid.NewGuid();
        var project = ProjectBuilder.OwnedBy(ownerId);
        var command = new CreateTaskCommand(
            Title: "Implement feature",
            Description: "Details here",
            Priority: TaskPriority.High,
            DueDate: DateTime.UtcNow.AddDays(5),
            ProjectId: project.Id,
            RequestingUserId: ownerId);

        _fixture.ProjectRepository
            .Setup(r => r.GetByIdWithTasksAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Title.Should().Be("Implement feature");
        result.Priority.Should().Be("High");
        result.Status.Should().Be("Todo");
        result.ProjectId.Should().Be(project.Id);
    }

    [Fact]
    public async Task Handle_WithNonExistentProject_ShouldThrowNotFoundException()
    {
        var command = new CreateTaskCommand(
            "Task", null, TaskPriority.Low, null, Guid.NewGuid(), Guid.NewGuid());

        _fixture.ProjectRepository
            .Setup(r => r.GetByIdWithTasksAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Project?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnProject_ShouldThrowForbiddenException()
    {
        var project = ProjectBuilder.OwnedBy(Guid.NewGuid());
        var command = new CreateTaskCommand(
            "Task", null, TaskPriority.Low, null, project.Id, Guid.NewGuid());

        _fixture.ProjectRepository
            .Setup(r => r.GetByIdWithTasksAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ShouldInvalidateTaskCache()
    {
        var ownerId = Guid.NewGuid();
        var project = ProjectBuilder.OwnedBy(ownerId);
        var command = new CreateTaskCommand(
            "Task", null, TaskPriority.Medium, null, project.Id, ownerId);

        _fixture.ProjectRepository
            .Setup(r => r.GetByIdWithTasksAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        await _handler.Handle(command, CancellationToken.None);

        _fixture.CacheService.Verify(
            c => c.RemoveAsync(
                $"tasks:project:{project.Id}",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}