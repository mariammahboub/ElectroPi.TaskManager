using ElectroPi.TaskManager.Application.Features.Auth.Commands.Register;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Auth;

public sealed class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    private static RegisterCommand ValidCommand() => new(
        "Jane Doe",
        "jane@electropi.com",
        "StrongPass@1",
        "StrongPass@1");


    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyFullName_ShouldHaveError(string fullName)
    {
        var result = _validator.TestValidate(ValidCommand() with { FullName = fullName });
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void Validate_WithFullNameOver100Chars_ShouldHaveError()
    {
        var longName = new string('a', 101);
        var result = _validator.TestValidate(ValidCommand() with { FullName = longName });
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }


    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    [InlineData("")]
    public void Validate_WithInvalidEmail_ShouldHaveError(string email)
    {
        var result = _validator.TestValidate(ValidCommand() with { Email = email });
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }


    [Theory]
    [InlineData("short")]
    [InlineData("nouppercase1@")]  
    [InlineData("NOLOWERCASE1@")]  
    [InlineData("NoSpecialChar1")]  
    [InlineData("NoDigit@Pass")]    
    public void Validate_WithWeakPassword_ShouldHaveError(string password)
    {
        var result = _validator.TestValidate(
            ValidCommand() with { Password = password, ConfirmPassword = password });
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }


    [Fact]
    public void Validate_WhenPasswordsDoNotMatch_ShouldHaveError()
    {
        var result = _validator.TestValidate(
            ValidCommand() with { ConfirmPassword = "DifferentPass@1" });
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Fact]
    public void Validate_WhenPasswordsMatch_ShouldNotHaveConfirmPasswordError()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword);
    }
}