using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Tests.xUnit.Integration.Repositories
{
    public class UserRepositoryIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepository _userRepository;

        public UserRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddAsync_WithValidUser_AddsUserToDatabase()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                Role = "User"
            };

            // Act
            await _userRepository.AddAsync(user);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _userRepository.GetByIdAsync(user.Id);
            result.Should().NotBeNull();
            result!.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleUsers_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Name = "User1", Email = "user1@example.com", PasswordHash = "hash", Role = "User" },
                new User { Name = "User2", Email = "user2@example.com", PasswordHash = "hash", Role = "Admin" }
            };

            foreach (var user in users)
            {
                await _userRepository.AddAsync(user);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetAllAsync();

            // Assert
            result.Should().HaveCountGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task IsEmailTakenAsync_WithExistingEmail_ReturnsTrue()
        {
            // Arrange
            var email = "existing@example.com";
            var user = new User
            {
                Name = "Test User",
                Email = email,
                PasswordHash = "hashed_password",
                Role = "User"
            };

            await _userRepository.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.IsEmailTakenAsync(email);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsEmailTakenAsync_WithNonExistingEmail_ReturnsFalse()
        {
            // Arrange
            var email = "nonexisting@example.com";

            // Act
            var result = await _userRepository.IsEmailTakenAsync(email);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetPagedUsersAsync_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            for (int i = 1; i <= 25; i++)
            {
                var user = new User
                {
                    Name = $"User {i}",
                    Email = $"user{i}@example.com",
                    PasswordHash = "hash",
                    Role = "User"
                };
                await _userRepository.AddAsync(user);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetPagedUsersAsync(1, 10, null);

            // Assert
            result.Should().HaveCount(10);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
