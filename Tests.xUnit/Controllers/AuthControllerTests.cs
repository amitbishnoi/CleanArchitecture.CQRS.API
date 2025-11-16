using API.Controllers;
using Application.Features.Authentication.Command;
using Application.Features.Authentication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Tests.xUnit.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new AuthController(_mockMediator.Object);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenNull()
        {
            _mockMediator.Setup(m => m.Send<AuthResponse?>(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AuthResponse?)null);

            var cmd = new LoginCommand { Email = "a@b.com", Password = "p" };
            var result = await _controller.Login(cmd);
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenValid()
        {
            var resp = new AuthResponse { Token = "t", ExpiresAt = System.DateTime.UtcNow };
            _mockMediator.Setup(m => m.Send<AuthResponse?>(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AuthResponse?)resp);

            var cmd = new LoginCommand { Email = "a@b.com", Password = "p" };
            var result = await _controller.Login(cmd);
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
