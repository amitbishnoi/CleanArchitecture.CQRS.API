using API.Controllers.V1;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Tests.xUnit.Controllers.V1
{
    public class ApiInfoControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly ApiInfoController _controller;

        public ApiInfoControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new ApiInfoController(_mockConfiguration.Object);
        }

        [Fact]
        public void GetApiInfo_ReturnsOkResult()
        {
            // Act
            var result = _controller.GetApiInfo();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetApiInfo_ReturnsCorrectVersion()
        {
            // Act
            var result = _controller.GetApiInfo() as OkObjectResult;
            
            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void GetApiInfo_ContainsRequiredProperties()
        {
            // Act
            var result = _controller.GetApiInfo() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            var response = result!.Value;
            response.Should().NotBeNull();
            response!.GetType().GetProperty("Success").Should().NotBeNull();
            response.GetType().GetProperty("StatusCode").Should().NotBeNull();
            response.GetType().GetProperty("Message").Should().NotBeNull();
            response.GetType().GetProperty("Data").Should().NotBeNull();
        }

        [Fact]
        public void GetHealth_ReturnsOkResult()
        {
            // Act
            var result = _controller.GetHealth();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetHealth_ReturnsHealthyStatus()
        {
            // Act
            var result = _controller.GetHealth() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);
        }

        [Fact]
        public void GetHealth_ContainsRequiredHealthData()
        {
            // Act
            var result = _controller.GetHealth() as OkObjectResult;
            var response = result!.Value;

            // Assert
            response.Should().NotBeNull();
            response!.GetType().GetProperty("Data").Should().NotBeNull();
        }
    }
}
