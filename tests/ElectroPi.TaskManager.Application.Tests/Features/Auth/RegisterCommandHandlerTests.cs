using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Features.Auth.Commands.Register;
using ElectroPi.TaskManager.Application.Features.Auth.DTOs;
using ElectroPi.TaskManager.Application.Tests.Common.Fixtures;
using FluentAssertions;
using Moq;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Auth;

public sealed class RegisterCommandHandlerTests
{
    private readonly MediatorFixture _fixture = new();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_fixture.AuthService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnAuthResponse()
    {
        var command = new RegisterCommand(
            "Jane Doe", "jane@electropi.com", "Pass@1234", "Pass@1234");

        var expectedResponse = new AuthResponseDto(
            Guid.NewGuid(), "Jane Doe", "jane@electropi.com",
            "Member", "jwt.token.here", DateTime.UtcNow.AddHours(1));

        _fixture.AuthService
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Email.Should().Be("jane@electropi.com");
        result.FullName.Should().Be("Jane Doe");
        result.Token.Should().Be("jwt.token.here");
        result.Role.Should().Be("Member");
    }

    [Fact]
    public async Task Handle_WhenAuthServiceThrowsConflict_ShouldPropagateException()
    {
        var command = new RegisterCommand(
            "Jane Doe", "jane@electropi.com", "Pass@1234", "Pass@1234");

        _fixture.AuthService
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflictException(nameof(Domain.Entities.ApplicationUser),
                "Email already registered."));

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectDtoToAuthService()
    {
        var command = new RegisterCommand(
            "Jane Doe", "jane@electropi.com", "Pass@1234", "Pass@1234");

        RegisterRequestDto? capturedDto = null;

        _fixture.AuthService
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<CancellationToken>()))
            .Callback<RegisterRequestDto, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(new AuthResponseDto(
                Guid.NewGuid(), "Jane Doe", "jane@electropi.com",
                "Member", "token", DateTime.UtcNow.AddHours(1)));

        await _handler.Handle(command, CancellationToken.None);

        capturedDto.Should().NotBeNull();
        capturedDto!.FullName.Should().Be(command.FullName);
        capturedDto.Email.Should().Be(command.Email);
        capturedDto.Password.Should().Be(command.Password);
        capturedDto.ConfirmPassword.Should().Be(command.ConfirmPassword);
    }
}