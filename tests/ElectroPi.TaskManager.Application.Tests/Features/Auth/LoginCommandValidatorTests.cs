using ElectroPi.TaskManager.Application.Features.Auth.Commands.Login;
using FluentValidation.TestHelper;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Auth;

public sealed class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    private static LoginCommand ValidCommand() =>
        new("jane@electropi.com", "AnyPassword@1");

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
        => _validator.TestValidate(ValidCommand()).ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData("", "Password")]
    [InlineData("notanemail", "Password")]
    public void Validate_WithInvalidEmail_ShouldHaveEmailError(string email, string _)
        => _validator.TestValidate(ValidCommand() with { Email = email })
                     .ShouldHaveValidationErrorFor(x => x.Email);

    [Fact]
    public void Validate_WithEmptyPassword_ShouldHavePasswordError()
        => _validator.TestValidate(ValidCommand() with { Password = "" })
                     .ShouldHaveValidationErrorFor(x => x.Password);
}