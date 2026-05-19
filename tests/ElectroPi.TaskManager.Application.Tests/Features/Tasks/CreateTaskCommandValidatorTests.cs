using ElectroPi.TaskManager.Application.Features.Tasks.Commands.CreateTask;
using ElectroPi.TaskManager.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Tasks;

public sealed class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator = new();

    private static CreateTaskCommand ValidCommand() => new(
        Title: "Valid Task",
        Description: "Some description",
        Priority: TaskPriority.Medium,
        DueDate: DateTime.UtcNow.AddDays(7),
        ProjectId: Guid.NewGuid(),
        RequestingUserId: Guid.NewGuid());

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
        => _validator.TestValidate(ValidCommand()).ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyTitle_ShouldHaveError(string title)
        => _validator.TestValidate(ValidCommand() with { Title = title })
                     .ShouldHaveValidationErrorFor(x => x.Title);

    [Fact]
    public void Validate_WithTitleExceeding300Chars_ShouldHaveError()
        => _validator.TestValidate(ValidCommand() with { Title = new string('x', 301) })
                     .ShouldHaveValidationErrorFor(x => x.Title);

    [Fact]
    public void Validate_WithDescriptionExceeding2000Chars_ShouldHaveError()
        => _validator.TestValidate(ValidCommand() with { Description = new string('x', 2001) })
                     .ShouldHaveValidationErrorFor(x => x.Description);

    [Fact]
    public void Validate_WithPastDueDate_ShouldHaveError()
        => _validator.TestValidate(ValidCommand() with { DueDate = DateTime.UtcNow.AddDays(-1) })
                     .ShouldHaveValidationErrorFor(x => x.DueDate);

    [Fact]
    public void Validate_WithNullDueDate_ShouldNotHaveDueDateError()
        => _validator.TestValidate(ValidCommand() with { DueDate = null })
                     .ShouldNotHaveValidationErrorFor(x => x.DueDate);

    [Fact]
    public void Validate_WithEmptyProjectId_ShouldHaveError()
        => _validator.TestValidate(ValidCommand() with { ProjectId = Guid.Empty })
                     .ShouldHaveValidationErrorFor(x => x.ProjectId);

    [Fact]
    public void Validate_WithInvalidPriority_ShouldHaveError()
        => _validator.TestValidate(ValidCommand() with { Priority = (TaskPriority)99 })
                     .ShouldHaveValidationErrorFor(x => x.Priority);
}