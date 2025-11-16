using Application.Features.Users.Commands.CreateUser;
using Application.Interfaces;
using Application.Interfaces.SecurityInterface;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.xUnit.Features.Users
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockEmailService = new Mock<IEmailService>();
            _handler = new CreateUserCommandHandler(_mockUnitOfWork.Object, _mockPasswordHasher.Object, _mockEmailService.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_CreatesUserSuccessfully()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                Role = "User"
            };

            var hashedPassword = "hashed_password_here";
            _mockPasswordHasher
                .Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns(hashedPassword);

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) =>
                {
                    user.Id = 1;
                    return user;
                });

            _mockUnitOfWork
                .Setup(x => x.Users)
                .Returns(mockUserRepository.Object);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync())
                .ReturnsAsync(1);

            _mockEmailService
                .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeGreaterThan(0);
            _mockPasswordHasher.Verify(x => x.HashPassword(command.Password), Times.Once);
            _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidEmail_ValidationShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Name = "John Doe",
                Email = "invalid-email",
                Password = "SecurePass123!",
                Role = "User"
            };

            var validator = new CreateUserCommandValidator();

            // Act
            var validationResult = await validator.ValidateAsync(command);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email));
        }

        [Fact]
        public async Task Handle_WithEmptyName_ValidationShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Name = "",
                Email = "john@example.com",
                Password = "SecurePass123!",
                Role = "User"
            };

            var validator = new CreateUserCommandValidator();

            // Act
            var validationResult = await validator.ValidateAsync(command);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
        }

        [Fact]
        public async Task Handle_WithEmptyPassword_ValidationShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "",
                Role = "User"
            };

            var validator = new CreateUserCommandValidator();

            // Act
            var validationResult = await validator.ValidateAsync(command);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
        }
    }
}
