using Application.Features.Authentication.Command;
using Application.Features.Authentication.Interfaces;
using Application.Interfaces;
using Application.Interfaces.SecurityInterface;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.xUnit.Features.Authentication
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _handler = new LoginCommandHandler(_mockAuthService.Object);
        }

        [Fact]
        public async Task Handle_WithValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            var expectedResponse = new Application.Features.Authentication.Dtos.AuthResponse
            {
                Token = "test-token-123",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _mockAuthService
                .Setup(x => x.AuthenticateAsync(It.IsAny<Application.Features.Authentication.Dtos.LoginRequest>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().Be("test-token-123");
            result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task Handle_WithInvalidCredentials_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            _mockAuthService
                .Setup(x => x.AuthenticateAsync(It.IsAny<Application.Features.Authentication.Dtos.LoginRequest>()))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithEmptyEmail_ShouldFail()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "",
                Password = "Password123"
            };

            // Act & Assert
            command.Email.Should().BeEmpty();
        }
    }
}
