using Application.Features.Courses.Commands.CreateCourse;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.xUnit.Features.Courses
{
    public class CreateCourseCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateCourseCommandHandler _handler;

        public CreateCourseCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new CreateCourseCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_CreatesCourseSuccessfully()
        {
            // Arrange
            var instructorId = 1;
            var instructor = new User 
            { 
                Id = instructorId, 
                Name = "Dr. Smith", 
                Email = "smith@example.com", 
                PasswordHash = "hashed", 
                Role = "Instructor" 
            };

            var command = new CreateCourseCommand
            {
                Title = "C# Advanced",
                Description = "Advanced C# programming course",
                InstructorId = instructorId
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(x => x.GetByIdAsync(instructorId))
                .ReturnsAsync(instructor);

            var mockCourseRepository = new Mock<ICourseRepository>();
            mockCourseRepository
                .Setup(x => x.AddAsync(It.IsAny<Course>()))
                .ReturnsAsync((Course course) =>
                {
                    course.Id = 1;
                    return course;
                });

            _mockUnitOfWork
                .Setup(x => x.Users)
                .Returns(mockUserRepository.Object);

            _mockUnitOfWork
                .Setup(x => x.Courses)
                .Returns(mockCourseRepository.Object);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
            mockUserRepository.Verify(x => x.GetByIdAsync(instructorId), Times.Once);
            mockCourseRepository.Verify(x => x.AddAsync(It.IsAny<Course>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidInstructor_ThrowsApplicationException()
        {
            // Arrange
            var invalidInstructorId = 999;
            var command = new CreateCourseCommand
            {
                Title = "C# Advanced",
                Description = "Advanced C# programming course",
                InstructorId = invalidInstructorId
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(x => x.GetByIdAsync(invalidInstructorId))
                .ReturnsAsync((User?)null);

            _mockUnitOfWork
                .Setup(x => x.Users)
                .Returns(mockUserRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Application.Common.Exceptions.ApplicationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithEmptyTitle_ValidationShouldFail()
        {
            // Arrange
            var command = new CreateCourseCommand
            {
                Title = "",
                Description = "Advanced C# programming course",
                InstructorId = 1
            };

            var validator = new CreateCourseCommandValidator();

            // Act
            var validationResult = await validator.ValidateAsync(command);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(command.Title));
        }

        [Fact]
        public async Task Handle_WithInvalidInstructorId_ValidationShouldFail()
        {
            // Arrange
            var command = new CreateCourseCommand
            {
                Title = "C# Advanced",
                Description = "Advanced C# programming course",
                InstructorId = 0
            };

            var validator = new CreateCourseCommandValidator();

            // Act
            var validationResult = await validator.ValidateAsync(command);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(command.InstructorId));
        }
    }
}
