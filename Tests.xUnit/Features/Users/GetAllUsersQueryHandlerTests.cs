using Application.Features.Users.Queries.GetAllUsers;
using Application.Features.Users.Dtos;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;
using Application.Common.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Tests.xUnit.Features.Users
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetAllUsersQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithUsers_ReturnsUserDtos()
        {
            // Arrange
            var users = new List<User>
            {
                new User 
                { 
                    Id = 1, 
                    Name = "John Doe", 
                    Email = "john@example.com", 
                    PasswordHash = "hashed", 
                    Role = "User" 
                },
                new User 
                { 
                    Id = 2, 
                    Name = "Jane Smith", 
                    Email = "jane@example.com", 
                    PasswordHash = "hashed", 
                    Role = "Admin" 
                }
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(x => x.GetPagedUsersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()))
                .ReturnsAsync(users.AsReadOnly());

            _mockUnitOfWork
                .Setup(x => x.Users)
                .Returns(mockUserRepository.Object);

            var query = new GetAllUsersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("John Doe");
            result[0].Email.Should().Be("john@example.com");
            result[0].Role.Should().Be("User");
            result.Should().NotContain(u => u.GetType().GetProperty("Password") != null);
        }

        [Fact]
        public async Task Handle_WithNoUsers_ReturnsEmptyList()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(x => x.GetPagedUsersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()))
                .ReturnsAsync(new List<User>().AsReadOnly());

            _mockUnitOfWork
                .Setup(x => x.Users)
                .Returns(mockUserRepository.Object);

            var query = new GetAllUsersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WithSearchTerm_FiltersBySearchTerm()
        {
            // Arrange
            var searchTerm = "John";
            var users = new List<User>
            {
                new User 
                { 
                    Id = 1, 
                    Name = "John Doe", 
                    Email = "john@example.com", 
                    PasswordHash = "hashed", 
                    Role = "User" 
                }
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(x => x.GetPagedUsersAsync(It.IsAny<int>(), It.IsAny<int>(), It.Is<string?>(s => s == searchTerm)))
                .ReturnsAsync(users.AsReadOnly());

            _mockUnitOfWork
                .Setup(x => x.Users)
                .Returns(mockUserRepository.Object);

            var query = new GetAllUsersQuery 
            { 
                Pagination = new PaginationParams { SearchTerm = searchTerm } 
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result[0].Name.Should().Contain("John");
        }
    }
}
