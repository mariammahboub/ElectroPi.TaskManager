using ElectroPi.TaskManager.Application.Features.Projects.Queries.GetAllProjects;
using ElectroPi.TaskManager.Application.Tests.Common.Fixtures;
using ElectroPi.TaskManager.Application.Tests.Helpers;
using FluentAssertions;
using Mapster;
using Moq;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Projects;

public sealed class GetAllProjectsQueryHandlerTests
{
    private readonly MediatorFixture _fixture = new();
    private readonly GetAllProjectsQueryHandler _handler;

    public GetAllProjectsQueryHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(GetAllProjectsQuery).Assembly);

        _handler = new GetAllProjectsQueryHandler(_fixture.UnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnProjectsList()
    {
        var ownerId = Guid.NewGuid();
        var project = ProjectBuilder.OwnedBy(ownerId);
        var query = new GetAllProjectsQuery(ownerId);

        _fixture.ProjectRepository
            .Setup(r => r.GetAllByOwnerAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { project });

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result.First().Id.Should().Be(project.Id);
    }
}