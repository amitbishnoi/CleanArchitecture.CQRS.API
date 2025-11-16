using Application.Features.Courses.Queries.GetAllCourses;
using Application.Features.Courses.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;
using Application.Common.Models;
using Application.Mappings;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Tests.xUnit.Features.Courses
{
    public class GetAllCoursesQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IMapper _mapper;
        private readonly GetAllCoursesQueryHandler _handler;

        public GetAllCoursesQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            // Setup mapping from Course -> CourseDto for collections
            mockMapper
                .Setup(m => m.Map<IReadOnlyList<CourseDto>>(It.IsAny<IReadOnlyList<Course>>()))
                .Returns((IReadOnlyList<Course> src) =>
                {
                    var list = new List<CourseDto>();
                    foreach (var c in src)
                    {
                            list.Add(new CourseDto { Id = c.Id, Title = c.Title, Description = c.Description });
                    }
                    return list as IReadOnlyList<CourseDto>;
                });

            _mapper = mockMapper.Object;
            _handler = new GetAllCoursesQueryHandler(_mockUnitOfWork.Object, _mapper);
        }

        [Fact]
        public async Task Handle_WithCourses_ReturnsCourses()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course 
                { 
                    Id = 1, 
                    Title = "C# Basics", 
                    Description = "Learn C# basics", 
                    InstructorId = 1 
                },
                new Course 
                { 
                    Id = 2, 
                    Title = "Advanced C#", 
                    Description = "Learn advanced C#", 
                    InstructorId = 1 
                }
            };

            var mockCourseRepository = new Mock<ICourseRepository>();
            mockCourseRepository
                .Setup(x => x.GetPagedCoursesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()))
                .ReturnsAsync(courses.AsReadOnly());

            _mockUnitOfWork
                .Setup(x => x.Courses)
                .Returns(mockCourseRepository.Object);

            var query = new GetAllCoursesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Title.Should().Be("C# Basics");
            result[1].Title.Should().Be("Advanced C#");
        }

        [Fact]
        public async Task Handle_WithNoCourses_ReturnsEmptyList()
        {
            // Arrange
            var mockCourseRepository = new Mock<ICourseRepository>();
            mockCourseRepository
                .Setup(x => x.GetPagedCoursesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()))
                .ReturnsAsync(new List<Course>().AsReadOnly());

            _mockUnitOfWork
                .Setup(x => x.Courses)
                .Returns(mockCourseRepository.Object);

            var query = new GetAllCoursesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WithSearchTerm_FiltersByTitle()
        {
            // Arrange
            var searchTerm = "C#";
            var courses = new List<Course>
            {
                new Course 
                { 
                    Id = 1, 
                    Title = "C# Basics", 
                    Description = "Learn C# basics", 
                    InstructorId = 1 
                }
            };

            var mockCourseRepository = new Mock<ICourseRepository>();
            mockCourseRepository
                .Setup(x => x.GetPagedCoursesAsync(It.IsAny<int>(), It.IsAny<int>(), It.Is<string?>(s => s == searchTerm)))
                .ReturnsAsync(courses.AsReadOnly());

            _mockUnitOfWork
                .Setup(x => x.Courses)
                .Returns(mockCourseRepository.Object);

            var query = new GetAllCoursesQuery 
            { 
                Pagination = new PaginationParams { SearchTerm = searchTerm } 
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result[0].Title.Should().Contain("C#");
        }
    }
}
