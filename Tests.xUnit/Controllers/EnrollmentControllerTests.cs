using API.Controllers;
using Application.Features.Enrollment.Queries.GetEnrollmentById;
using Application.Features.Enrollment.Commands.CreateEnrollment;
using Application.Features.Enrollment.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Tests.xUnit.Controllers
{
    public class EnrollmentControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly EnrollmentController _controller;

        public EnrollmentControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new EnrollmentController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetEnrollmentById_ReturnsNotFound_WhenNull()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetEnrollmentByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EnrollmentDto?)null);

            var result = await _controller.GetEnrollmentById(5);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetEnrollmentById_ReturnsOk_WhenPresent()
        {
            var dto = new EnrollmentDto { Id = 1, UserName = "John", CourseTitle = "C#" };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetEnrollmentByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            var result = await _controller.GetEnrollmentById(1);
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
