using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Features.Projects.Commands.CreateProject;
using ElectroPi.TaskManager.Application.Tests.Common.Fixtures;
using ElectroPi.TaskManager.Domain.Entities;
using FluentAssertions;
using Mapster;
using Moq;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Projects;

public sealed class CreateProjectCommandHandlerTests
{
    private readonly MediatorFixture _fixture = new();
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(CreateProjectCommand).Assembly);

        _handler = new CreateProjectCommandHandler(
            _fixture.UnitOfWork.Object,
            _fixture.CacheService.Object);
    }
    private static CreateProjectCommand ValidCommand(Guid? ownerId = null) => new(
        Name: "New Project",
        Description: "A great project",
        OwnerId: ownerId ?? Guid.NewGuid());

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnProjectDto()
    {
        var command = ValidCommand();

        _fixture.ProjectRepository
            .Setup(r => r.NameExistsForOwnerAsync(command.Name, command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _fixture.ProjectRepository
            .Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_WithDuplicateProjectName_ShouldThrowConflictException()
    {
        var command = ValidCommand();

        _fixture.ProjectRepository
            .Setup(r => r.NameExistsForOwnerAsync(command.Name, command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ShouldCallSaveChangesOnce()
    {
        var command = ValidCommand();

        _fixture.ProjectRepository
            .Setup(r => r.NameExistsForOwnerAsync(command.Name, command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _fixture.ProjectRepository
            .Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        _fixture.UnitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ShouldInvalidateOwnerCache()
    {
        var command = ValidCommand();

        _fixture.ProjectRepository
            .Setup(r => r.NameExistsForOwnerAsync(command.Name, command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _fixture.ProjectRepository
            .Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        _fixture.CacheService.Verify(
            c => c.RemoveByPrefixAsync(
                $"projects:user:{command.OwnerId}",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNameConflictThrows_ShouldNotCallSaveChanges()
    {
        var command = ValidCommand();

        _fixture.ProjectRepository
            .Setup(r => r.NameExistsForOwnerAsync(command.Name, command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        try { await _handler.Handle(command, CancellationToken.None); } catch {}

        _fixture.UnitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}