using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Features.Projects.Commands.DeleteProject;
using ElectroPi.TaskManager.Application.Tests.Common.Fixtures;
using ElectroPi.TaskManager.Application.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Projects;

public sealed class DeleteProjectCommandHandlerTests
{
    private readonly MediatorFixture _fixture = new();
    private readonly DeleteProjectCommandHandler _handler;

    public DeleteProjectCommandHandlerTests()
        => _handler = new DeleteProjectCommandHandler(
            _fixture.UnitOfWork.Object,
            _fixture.CacheService.Object);

    [Fact]
    public async Task Handle_WithValidOwner_ShouldDeleteAndSave()
    {
        var ownerId = Guid.NewGuid();
        var project = ProjectBuilder.OwnedBy(ownerId);
        var command = new DeleteProjectCommand(project.Id, ownerId);

        _fixture.ProjectRepository
            .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        await _handler.Handle(command, CancellationToken.None);

        _fixture.ProjectRepository.Verify(r => r.Delete(project), Times.Once);
        _fixture.UnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentProject_ShouldThrowNotFoundException()
    {
        var command = new DeleteProjectCommand(Guid.NewGuid(), Guid.NewGuid());

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
        var command = new DeleteProjectCommand(project.Id, Guid.NewGuid());

        _fixture.ProjectRepository
            .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_WhenForbidden_ShouldNotCallDelete()
    {
        var project = ProjectBuilder.OwnedBy(Guid.NewGuid());
        var command = new DeleteProjectCommand(project.Id, Guid.NewGuid());

        _fixture.ProjectRepository
            .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        _fixture.ProjectRepository.Verify(r => r.Delete(It.IsAny<Domain.Entities.Project>()), Times.Never);
    }
}