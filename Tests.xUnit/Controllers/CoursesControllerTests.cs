using API.Controllers;
using Application.Features.Courses.Queries.GetCourseById;
using Application.Features.Courses.Queries.GetAllCourses;
using Application.Features.Courses.Commands.CreateCourse;
using Application.Features.Courses.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Tests.xUnit.Controllers
{
    public class CoursesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly CoursesController _controller;

        public CoursesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new CoursesController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetCourseById_ReturnsNotFound_WhenCourseIsNull()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCourseByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseDto?)null);

            var result = await _controller.GetCourseById(999);

            result.Should().BeOfType<NotFoundObjectResult>();
            var nf = result as NotFoundObjectResult;
            nf!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetCourseById_ReturnsOk_WhenCourseExists()
        {
            var dto = new CourseDto { Id = 1, Title = "C#", Description = "desc" };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCourseByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            var result = await _controller.GetCourseById(1);

            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(dto);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithList()
        {
            var list = new List<CourseDto> { new CourseDto { Id = 1, Title = "C#" } };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllCoursesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<CourseDto>)list);

            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<CreateCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            var cmd = new CreateCourseCommand { Title = "x", Description = "d", InstructorId = 1 };
            var result = await _controller.Create(cmd);

            result.Should().BeOfType<CreatedAtActionResult>();
            var created = result as CreatedAtActionResult;
            created!.StatusCode.Should().Be(201);
        }
    }
}
