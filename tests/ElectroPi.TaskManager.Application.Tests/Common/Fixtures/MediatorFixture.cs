using ElectroPi.TaskManager.Application.Common.Interfaces;
using ElectroPi.TaskManager.Domain.Interfaces;
using ElectroPi.TaskManager.Domain.Repositories;
using Moq;

namespace ElectroPi.TaskManager.Application.Tests.Common.Fixtures;

public sealed class MediatorFixture
{
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IProjectRepository> ProjectRepository { get; } = new();
    public Mock<ITaskRepository> TaskRepository { get; } = new();
    public Mock<IUserRepository> UserRepository { get; } = new();
    public Mock<ICacheService> CacheService { get; } = new();
    public Mock<IAuthService> AuthService { get; } = new();
    public Mock<IJwtTokenService> JwtTokenService { get; } = new();

    public MediatorFixture()
    {
        UnitOfWork.Setup(u => u.Projects).Returns(ProjectRepository.Object);
        UnitOfWork.Setup(u => u.Tasks).Returns(TaskRepository.Object);
        UnitOfWork.Setup(u => u.Users).Returns(UserRepository.Object);
        UnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(1);


        CacheService
            .Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        CacheService
            .Setup(c => c.RemoveByPrefixAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }
}