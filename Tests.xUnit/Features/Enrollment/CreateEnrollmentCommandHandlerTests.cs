using Application.Features.Enrollment.Commands.CreateEnrollment;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.xUnit.Features.Enrollment
{
    public class CreateEnrollmentCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateEnrollmentCommandHandler _handler;

        public CreateEnrollmentCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new CreateEnrollmentCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_CreatesEnrollmentSuccessfully()
        {
            // Arrange
            var userId = 1;
            var courseId = 1;

            var command = new CreateEnrollmentCommand
            {
                UserId = userId,
                CourseId = courseId
            };

            var user = new User 
            { 
                Id = userId, 
                Name = "John Doe", 
                Email = "john@example.com", 
                PasswordHash = "hashed", 
                Role = "User" 
            };

            var course = new Course 
            { 
                Id = courseId, 
                Title = "C# Basics", 
                Description = "Learn C#", 
                InstructorId = 1 
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var mockCourseRepository = new Mock<ICourseRepository>();
            mockCourseRepository
                .Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            var mockEnrollmentRepository = new Mock<IEnrollmentRepository>();
            mockEnrollmentRepository
                .Setup(x => x.ExistsAsync(userId, courseId))
                .ReturnsAsync(false);

            mockEnrollmentRepository
                .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Enrollment>()))
                .ReturnsAsync((Domain.Entities.Enrollment enrollment) =>
                {
                    enrollment.Id = 1;
                    return enrollment;
                });

            _mockUnitOfWork.Setup(x => x.Users).Returns(mockUserRepository.Object);
            _mockUnitOfWork.Setup(x => x.Courses).Returns(mockCourseRepository.Object);
            _mockUnitOfWork.Setup(x => x.Enrollment).Returns(mockEnrollmentRepository.Object);
            _mockUnitOfWork.Setup(x => x.SaveAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Handle_WithDuplicateEnrollment_ReturnZero()
        {
            // Arrange
            var userId = 1;
            var courseId = 1;

            var command = new CreateEnrollmentCommand
            {
                UserId = userId,
                CourseId = courseId
            };

            var mockEnrollmentRepository = new Mock<IEnrollmentRepository>();
            mockEnrollmentRepository
                .Setup(x => x.ExistsAsync(userId, courseId))
                .ReturnsAsync(true);

            // Ensure user and course repos exist so handler validation doesn't NRE
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId, Name = "John", Email = "john@example.com", PasswordHash = "h", Role = "User" });
            var mockCourseRepository = new Mock<ICourseRepository>();
            mockCourseRepository.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(new Course { Id = courseId, Title = "C#", Description = "d", InstructorId = 1 });

            _mockUnitOfWork.Setup(x => x.Users).Returns(mockUserRepository.Object);
            _mockUnitOfWork.Setup(x => x.Courses).Returns(mockCourseRepository.Object);
            _mockUnitOfWork.Setup(x => x.Enrollment).Returns(mockEnrollmentRepository.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task Handle_WithInvalidUser_ThrowsException()
        {
            // Arrange
            var userId = 999;
            var courseId = 1;

            var command = new CreateEnrollmentCommand
            {
                UserId = userId,
                CourseId = courseId
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            _mockUnitOfWork.Setup(x => x.Users).Returns(mockUserRepository.Object);

            // Ensure Enrollment repo is present so handler doesn't NRE when calling ExistsAsync
            var mockEnrollmentRepository = new Mock<IEnrollmentRepository>();
            mockEnrollmentRepository.Setup(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
            _mockUnitOfWork.Setup(x => x.Enrollment).Returns(mockEnrollmentRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Application.Common.Exceptions.ApplicationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
