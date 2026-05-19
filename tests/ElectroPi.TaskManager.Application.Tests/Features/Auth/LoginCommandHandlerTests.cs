using ElectroPi.TaskManager.Application.Common.Exceptions;
using ElectroPi.TaskManager.Application.Features.Auth.Commands.Login;
using ElectroPi.TaskManager.Application.Features.Auth.DTOs;
using ElectroPi.TaskManager.Application.Tests.Common.Fixtures;
using FluentAssertions;
using Moq;
using Xunit;

namespace ElectroPi.TaskManager.Application.Tests.Features.Auth;

public sealed class LoginCommandHandlerTests
{
    private readonly MediatorFixture _fixture = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_fixture.AuthService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnAuthResponse()
    {
        var command = new LoginCommand("jane@electropi.com", "Pass@1234");

        var expectedResponse = new AuthResponseDto(
            Guid.NewGuid(), "Jane Doe", "jane@electropi.com",
            "Member", "jwt.token.here", DateTime.UtcNow.AddHours(1));

        _fixture.AuthService
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Email.Should().Be("jane@electropi.com");
        result.Token.Should().Be("jwt.token.here");
    }

    [Fact]
    public async Task Handle_WhenAuthServiceThrowsUnauthorized_ShouldPropagateException()
    {
        var command = new LoginCommand("jane@electropi.com", "WrongPass");

        // هنا بنفترض إن السيرفيس بترمي UnauthorizedException أو BadRequestException لو الباسورد غلط
        _fixture.AuthService
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedException("Invalid email or password."));

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectDtoToAuthService()
    {
        var command = new LoginCommand("jane@electropi.com", "Pass@1234");

        LoginRequestDto? capturedDto = null;

        _fixture.AuthService
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .Callback<LoginRequestDto, CancellationToken>((dto, _) => capturedDto = dto)
            .ReturnsAsync(new AuthResponseDto(
                Guid.NewGuid(), "Jane Doe", "jane@electropi.com",
                "Member", "token", DateTime.UtcNow.AddHours(1)));

        await _handler.Handle(command, CancellationToken.None);

        capturedDto.Should().NotBeNull();
        capturedDto!.Email.Should().Be(command.Email);
        capturedDto.Password.Should().Be(command.Password);
    }
}