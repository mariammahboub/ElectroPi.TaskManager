using ElectroPi.TaskManager.Application.Tests.Helpers;
using ElectroPi.TaskManager.Domain.Enums;
using ElectroPi.TaskManager.Domain.Errors;
using FluentAssertions;
using Xunit;

namespace ElectroPi.TaskManager.Domain.Tests.Entities;

public sealed class ProjectTaskTests
{

    [Fact]
    public void NewTask_ShouldHaveStatusTodo()
    {
        var (_, task) = ProjectTaskBuilder.Default();

        task.Status.Should().Be(ProjectTaskStatus.Todo);
    }

    [Fact]
    public void NewTask_ShouldHaveCorrectTitle()
    {
        var (_, task) = new ProjectTaskBuilder().WithTitle("My Task").Build();

        task.Title.Should().Be("My Task");
    }

    [Fact]
    public void NewTask_DueDateShouldBeUtc()
    {
        var localDue = new DateTime(2025, 12, 31, 10, 0, 0, DateTimeKind.Local);
        var (_, task) = new ProjectTaskBuilder().WithDueDate(localDue).Build();

        task.DueDate.Should().NotBeNull();
        task.DueDate!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }


    [Fact]
    public void UpdateStatus_FromTodo_ToInProgress_ShouldSucceed()
    {
        var (_, task) = ProjectTaskBuilder.Default();

        task.UpdateStatus(ProjectTaskStatus.InProgress);

        task.Status.Should().Be(ProjectTaskStatus.InProgress);
    }

    [Fact]
    public void UpdateStatus_FromInProgress_ToDone_ShouldSucceed()
    {
        var (_, task) = ProjectTaskBuilder.Default();
        task.UpdateStatus(ProjectTaskStatus.InProgress);

        task.UpdateStatus(ProjectTaskStatus.Done);

        task.Status.Should().Be(ProjectTaskStatus.Done);
    }

    [Fact]
    public void UpdateStatus_FromTodo_ToDone_ShouldSucceed()
    {
        var (_, task) = ProjectTaskBuilder.Default();

        task.UpdateStatus(ProjectTaskStatus.Done);

        task.Status.Should().Be(ProjectTaskStatus.Done);
    }


    [Fact]
    public void UpdateStatus_ToSameStatus_ShouldThrowDomainError()
    {
        var (_, task) = ProjectTaskBuilder.Default();

        var act = () => task.UpdateStatus(ProjectTaskStatus.Todo);

        act.Should().Throw<DomainError>()
            .Which.Code.Should().Be(ErrorCodes.Task.InvalidStatusTransition);
    }

    [Fact]
    public void UpdateStatus_FromInProgress_ToTodo_ShouldThrowDomainError()
    {
        var (_, task) = ProjectTaskBuilder.Default();
        task.UpdateStatus(ProjectTaskStatus.InProgress);

        var act = () => task.UpdateStatus(ProjectTaskStatus.Todo);

        act.Should().Throw<DomainError>()
            .Which.Code.Should().Be(ErrorCodes.Task.InvalidStatusTransition);
    }

    [Fact]
    public void UpdateStatus_FromDone_ToInProgress_ShouldThrowDomainError()
    {
        var (_, task) = ProjectTaskBuilder.Default();
        task.UpdateStatus(ProjectTaskStatus.Done);

        var act = () => task.UpdateStatus(ProjectTaskStatus.InProgress);

        act.Should().Throw<DomainError>()
            .Which.Code.Should().Be(ErrorCodes.Task.InvalidStatusTransition);
    }


    [Fact]
    public void Update_ShouldMutateAllMutableFields()
    {
        var (_, task) = ProjectTaskBuilder.Default();
        var newDueDate = DateTime.UtcNow.AddDays(14);

        task.Update("Updated Title", "Updated description", TaskPriority.Critical, newDueDate);

        task.Title.Should().Be("Updated Title");
        task.Description.Should().Be("Updated description");
        task.Priority.Should().Be(TaskPriority.Critical);
        task.DueDate!.Value.Should().BeCloseTo(newDueDate.ToUniversalTime(), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_ShouldNotChangeStatus()
    {
        var (_, task) = ProjectTaskBuilder.Default();
        task.UpdateStatus(ProjectTaskStatus.InProgress);

        task.Update("New Title", null, TaskPriority.Low, null);

        task.Status.Should().Be(ProjectTaskStatus.InProgress);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithEmptyTitle_ShouldThrowArgumentException(string title)
    {
        var (_, task) = ProjectTaskBuilder.Default();

        var act = () => task.Update(title, null, TaskPriority.Medium, null);

        act.Should().Throw<ArgumentException>();
    }


    [Fact]
    public void IsOverdue_WhenDueDateIsInPastAndStatusIsNotDone_ShouldReturnTrue()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var (_, task) = new ProjectTaskBuilder().WithDueDate(pastDate).Build();

        task.IsOverdue().Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_WhenDueDateIsInPastButStatusIsDone_ShouldReturnFalse()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var (_, task) = new ProjectTaskBuilder().WithDueDate(pastDate).Build();
        task.UpdateStatus(ProjectTaskStatus.Done);

        task.IsOverdue().Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WhenDueDateIsInFuture_ShouldReturnFalse()
    {
        var futureDate = DateTime.UtcNow.AddDays(7);
        var (_, task) = new ProjectTaskBuilder().WithDueDate(futureDate).Build();

        task.IsOverdue().Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WhenNoDueDateSet_ShouldReturnFalse()
    {
        var (_, task) = new ProjectTaskBuilder().WithDueDate(null).Build();

        task.IsOverdue().Should().BeFalse();
    }
}