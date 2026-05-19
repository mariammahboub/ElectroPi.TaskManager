using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Features.Projects.Commands.UpdateProject;
using ElectroPi.TaskManager.Application.Tests.Common.Fixtures;
using ElectroPi.TaskManager.Application.Tests.Helpers;
using FluentAssertions;
using Mapster;
using Moq;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Projects;

public sealed class UpdateProjectCommandHandlerTests
{
    private readonly MediatorFixture _fixture = new();
    private readonly UpdateProjectCommandHandler _handler;

    public UpdateProjectCommandHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(UpdateProjectCommand).Assembly);

        _handler = new UpdateProjectCommandHandler(
            _fixture.UnitOfWork.Object,
            _fixture.CacheService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateAndSave()
    {
        var ownerId = Guid.NewGuid();
        var project = ProjectBuilder.OwnedBy(ownerId);
        var command = new UpdateProjectCommand(project.Id, "Updated Name", "Updated Desc", ownerId);

        _fixture.ProjectRepository
            .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _fixture.ProjectRepository
            .Setup(r => r.NameExistsForOwnerAsync(command.Name, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _handler.Handle(command, CancellationToken.None);

        project.Name.Should().Be("Updated Name");
        project.Description.Should().Be("Updated Desc");
        _fixture.UnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentProject_ShouldThrowNotFoundException()
    {
        var command = new UpdateProjectCommand(Guid.NewGuid(), "Name", "Desc", Guid.NewGuid());

        _fixture.ProjectRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Project?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnProject_ShouldThrowForbiddenException()
    {
        var project = ProjectBuilder.OwnedBy(Guid.NewGuid());
        var command = new UpdateProjectCommand(project.Id, "Name", "Desc", Guid.NewGuid()); 

        _fixture.ProjectRepository
            .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}