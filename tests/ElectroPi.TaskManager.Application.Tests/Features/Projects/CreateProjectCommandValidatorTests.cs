using ElectroPi.TaskManager.Application.Features.Projects.Commands.CreateProject;
using FluentValidation.TestHelper;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Projects;

public sealed class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator = new();

    private static CreateProjectCommand ValidCommand() =>
        new("Valid Project", "A description", Guid.NewGuid());

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
        => _validator.TestValidate(ValidCommand()).ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyName_ShouldHaveError(string name)
        => _validator.TestValidate(ValidCommand() with { Name = name })
                     .ShouldHaveValidationErrorFor(x => x.Name);

    [Fact]
    public void Validate_WithNameExceeding200Chars_ShouldHaveError()
        => _validator.TestValidate(ValidCommand() with { Name = new string('x', 201) })
                     .ShouldHaveValidationErrorFor(x => x.Name);

    [Fact]
    public void Validate_WithDescriptionExceeding1000Chars_ShouldHaveError()
        => _validator.TestValidate(ValidCommand() with { Description = new string('x', 1001) })
                     .ShouldHaveValidationErrorFor(x => x.Description);

    [Fact]
    public void Validate_WithNullDescription_ShouldNotHaveDescriptionError()
        => _validator.TestValidate(ValidCommand() with { Description = null })
                     .ShouldNotHaveValidationErrorFor(x => x.Description);

    [Fact]
    public void Validate_WithEmptyOwnerId_ShouldHaveError()
        => _validator.TestValidate(ValidCommand() with { OwnerId = Guid.Empty })
                     .ShouldHaveValidationErrorFor(x => x.OwnerId);
}