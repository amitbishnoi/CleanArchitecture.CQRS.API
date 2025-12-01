using API.Controllers.V2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Tests.xUnit.Controllers.V2
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
        public void GetApiInfo_ReturnsVersion2()
        {
            // Act
            var result = _controller.GetApiInfo() as OkObjectResult;
            
            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);
        }

        [Fact]
        public void GetApiInfo_ContainsEnhancedFeatures()
        {
            // Act
            var result = _controller.GetApiInfo() as OkObjectResult;
            var response = result!.Value;

            // Assert
            response.Should().NotBeNull();
            response!.GetType().GetProperty("Success").Should().NotBeNull();
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
        public void GetHealth_ReturnsDetailedHealthInfo()
        {
            // Act
            var result = _controller.GetHealth() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);
        }

        [Fact]
        public void GetHealth_ContainsSystemInfo()
        {
            // Act
            var result = _controller.GetHealth() as OkObjectResult;
            var response = result!.Value;

            // Assert
            response.Should().NotBeNull();
            response!.GetType().GetProperty("Data").Should().NotBeNull();
        }

        [Fact]
        public void GetMetrics_ReturnsOkResult()
        {
            // Act
            var result = _controller.GetMetrics();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetMetrics_ReturnsMetricsData()
        {
            // Act
            var result = _controller.GetMetrics() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void GetMetrics_ContainsRequiredMetrics()
        {
            // Act
            var result = _controller.GetMetrics() as OkObjectResult;
            var response = result!.Value;

            // Assert
            response.Should().NotBeNull();
            response!.GetType().GetProperty("Success").Should().NotBeNull();
            response.GetType().GetProperty("Data").Should().NotBeNull();
        }
    }
}
