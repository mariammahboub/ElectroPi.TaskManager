using ElectroPi.TaskManager.Application.Tests.Helpers;
using ElectroPi.TaskManager.Domain.Entities;
using ElectroPi.TaskManager.Domain.Enums;
using ElectroPi.TaskManager.Domain.Errors;
using FluentAssertions;
using Xunit;

namespace ElectroPi.TaskManager.Domain.Tests.Entities;

public sealed class ProjectTests
{

    [Fact]
    public void Create_WithValidInputs_ShouldReturnProject()
    {
        var ownerId = Guid.NewGuid();

        var project = Project.Create("My Project", "A description", ownerId);

        project.Name.Should().Be("My Project");
        project.Description.Should().Be("A description");
        project.OwnerId.Should().Be(ownerId);
        project.Tasks.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldTrimProjectName()
    {
        var project = Project.Create("  My Project  ", null, Guid.NewGuid());

        project.Name.Should().Be("My Project");
    }

    [Fact]
    public void Create_WithNullDescription_ShouldAllowNullDescription()
    {
        var project = Project.Create("My Project", null, Guid.NewGuid());

        project.Description.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrowArgumentException(string name)
    {
        var act = () => Project.Create(name, null, Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyOwnerId_ShouldThrowArgumentException()
    {
        var act = () => Project.Create("My Project", null, Guid.Empty);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*OwnerId*");
    }


    [Fact]
    public void Update_WithValidInputs_ShouldMutateNameAndDescription()
    {
        var project = ProjectBuilder.Default();

        project.Update("Updated Name", "Updated description");

        project.Name.Should().Be("Updated Name");
        project.Description.Should().Be("Updated description");
    }

    [Fact]
    public void Update_WithNullDescription_ShouldClearDescription()
    {
        var project = ProjectBuilder.Default();

        project.Update("Updated Name", null);

        project.Description.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithEmptyName_ShouldThrowArgumentException(string name)
    {
        var project = ProjectBuilder.Default();

        var act = () => project.Update(name, null);

        act.Should().Throw<ArgumentException>();
    }


    [Fact]
    public void IsOwnedBy_WithCorrectOwner_ShouldReturnTrue()
    {
        var ownerId = Guid.NewGuid();
        var project = ProjectBuilder.OwnedBy(ownerId);

        project.IsOwnedBy(ownerId).Should().BeTrue();
    }

    [Fact]
    public void IsOwnedBy_WithDifferentUser_ShouldReturnFalse()
    {
        var project = ProjectBuilder.Default();
        var differentId = Guid.NewGuid();

        project.IsOwnedBy(differentId).Should().BeFalse();
    }


    [Fact]
    public void AddTask_WithValidInputs_ShouldAddTaskToCollection()
    {
        var project = ProjectBuilder.Default();

        var task = project.AddTask("Write tests", "Write unit tests", TaskPriority.High, null);

        project.Tasks.Should().HaveCount(1);
        project.Tasks.First().Should().Be(task);
    }

    [Fact]
    public void AddTask_ShouldSetTaskStatusToTodo()
    {
        var project = ProjectBuilder.Default();

        var task = project.AddTask("Write tests", null, TaskPriority.Medium, null);

        task.Status.Should().Be(ProjectTaskStatus.Todo);
    }

    [Fact]
    public void AddTask_ShouldBindTaskToProjectId()
    {
        var project = ProjectBuilder.Default();

        var task = project.AddTask("Write tests", null, TaskPriority.Low, null);

        task.ProjectId.Should().Be(project.Id);
    }

    [Fact]
    public void AddTask_MultipleTasks_ShouldAllAppearInCollection()
    {
        var project = ProjectBuilder.Default();

        project.AddTask("Task One", null, TaskPriority.Low, null);
        project.AddTask("Task Two", null, TaskPriority.Medium, null);
        project.AddTask("Task Three", null, TaskPriority.High, null);

        project.Tasks.Should().HaveCount(3);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddTask_WithEmptyTitle_ShouldThrowArgumentException(string title)
    {
        var project = ProjectBuilder.Default();

        var act = () => project.AddTask(title, null, TaskPriority.Medium, null);

        act.Should().Throw<ArgumentException>();
    }


    [Fact]
    public void RemoveTask_WithExistingTask_ShouldRemoveFromCollection()
    {
        var project = ProjectBuilder.Default();
        var task = project.AddTask("Task to remove", null, TaskPriority.Low, null);

        project.RemoveTask(task.Id);

        project.Tasks.Should().BeEmpty();
    }

    [Fact]
    public void RemoveTask_WithNonExistentTaskId_ShouldThrowDomainError()
    {
        var project = ProjectBuilder.Default();
        var nonExistentId = Guid.NewGuid();

        var act = () => project.RemoveTask(nonExistentId);

        act.Should().Throw<DomainError>()
            .Which.Code.Should().Be(ErrorCodes.Task.NotFound);
    }

    [Fact]
    public void RemoveTask_ShouldOnlyRemoveTargetTask()
    {
        var project = ProjectBuilder.Default();
        var taskOne = project.AddTask("Keep me", null, TaskPriority.Low, null);
        var taskTwo = project.AddTask("Remove me", null, TaskPriority.Low, null);

        project.RemoveTask(taskTwo.Id);

        project.Tasks.Should().HaveCount(1);
        project.Tasks.First().Id.Should().Be(taskOne.Id);
    }
}