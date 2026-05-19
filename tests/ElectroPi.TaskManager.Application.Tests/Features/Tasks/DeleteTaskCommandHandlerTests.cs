using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Features.Tasks.Commands.DeleteTask;
using ElectroPi.TaskManager.Application.Tests.Common.Fixtures;
using ElectroPi.TaskManager.Application.Tests.Helpers;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Tasks
{
    public sealed class DeleteTaskCommandHandlerTest
    {
        private readonly MediatorFixture _fixture = new();
        private readonly DeleteTaskCommandHandler _handler;

        public DeleteTaskCommandHandlerTest()
            => _handler = new DeleteTaskCommandHandler(
                _fixture.UnitOfWork.Object,
                _fixture.CacheService.Object);

        [Fact]
        public async Task Handle_WithValidOwner_ShouldDeleteAndSave()
        {
            var ownerId = Guid.NewGuid();
            var (project, task) = new ProjectTaskBuilder().WithOwnerId(ownerId).Build();
            var command = new DeleteTaskCommand(task.Id, ownerId);

            _fixture.TaskRepository
                .Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);


            _fixture.ProjectRepository
                .Setup(r => r.GetByIdWithTasksAsync(project.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            _fixture.UnitOfWork.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentTask_ShouldThrowNotFoundException()
        {
            var command = new DeleteTaskCommand(Guid.NewGuid(), Guid.NewGuid());

            _fixture.TaskRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.ProjectTask?)null);

            var act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotOwnProject_ShouldThrowForbiddenException()
        {
            var (project, task) = new ProjectTaskBuilder().WithOwnerId(Guid.NewGuid()).Build();
            var command = new DeleteTaskCommand(task.Id, Guid.NewGuid()); 

            _fixture.TaskRepository
                .Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            _fixture.ProjectRepository
                .Setup(r => r.GetByIdWithTasksAsync(project.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_WhenForbidden_ShouldNotCallSave()
        {
            var (project, task) = new ProjectTaskBuilder().WithOwnerId(Guid.NewGuid()).Build();
            var command = new DeleteTaskCommand(task.Id, Guid.NewGuid());

            _fixture.TaskRepository
                .Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            _fixture.ProjectRepository
                .Setup(r => r.GetByIdWithTasksAsync(project.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);
            try { await _handler.Handle(command, CancellationToken.None); } catch { }

            _fixture.UnitOfWork.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}