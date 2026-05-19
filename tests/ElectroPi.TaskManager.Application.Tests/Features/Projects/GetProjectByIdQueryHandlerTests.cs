using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Features.Projects.Queries.GetProjectById;
using ElectroPi.TaskManager.Application.Tests.Common.Fixtures;
using ElectroPi.TaskManager.Application.Tests.Helpers;
using FluentAssertions;
using Mapster;
using Moq;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Projects
{
    public sealed class GetProjectByIdQueryHandlerTests
    {
        private readonly MediatorFixture _fixture = new();
        private readonly GetProjectByIdQueryHandler _handler;

        public GetProjectByIdQueryHandlerTests()
        {
            TypeAdapterConfig.GlobalSettings.Scan(
                typeof(Application.Common.Mappings.ProjectMappingConfig).Assembly);

            _handler = new GetProjectByIdQueryHandler(_fixture.UnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidQuery_ShouldReturnProject()
        {
            var ownerId = Guid.NewGuid();
            var project = ProjectBuilder.OwnedBy(ownerId);

            var query = new GetProjectByIdQuery(
                ProjectId: project.Id,
                RequestingUserId: ownerId);

 
            _fixture.ProjectRepository
                .Setup(r => r.GetByIdWithTasksAsync(project.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);  

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(project.Id);
            result.Name.Should().Be(project.Name);
        }

        [Fact]
        public async Task Handle_WithNonExistentProject_ShouldThrowNotFoundException()
        {
            var query = new GetProjectByIdQuery(Guid.NewGuid(), Guid.NewGuid());

            _fixture.ProjectRepository
                .Setup(r => r.GetByIdWithTasksAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Project?)null);

            var act = async () => await _handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotOwnProject_ShouldThrowForbiddenException()
        {
            var project = ProjectBuilder.OwnedBy(Guid.NewGuid());

            var query = new GetProjectByIdQuery(
                ProjectId: project.Id,
                RequestingUserId: Guid.NewGuid()); 

            _fixture.ProjectRepository
                .Setup(r => r.GetByIdWithTasksAsync(project.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            var act = async () => await _handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<ForbiddenException>();
        }
    }
}