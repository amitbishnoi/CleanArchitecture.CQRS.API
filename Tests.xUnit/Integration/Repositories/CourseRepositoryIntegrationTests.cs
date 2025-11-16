using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Tests.xUnit.Integration.Repositories
{
    public class CourseRepositoryIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CourseRepository _courseRepository;
        private readonly UserRepository _userRepository;

        public CourseRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _courseRepository = new CourseRepository(_context);
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddAsync_WithValidCourse_AddsCourseToDatabase()
        {
            // Arrange
            var instructor = new User
            {
                Name = "Dr. Smith",
                Email = "smith@example.com",
                PasswordHash = "hash",
                Role = "Instructor"
            };
            await _userRepository.AddAsync(instructor);
            await _context.SaveChangesAsync();

            var course = new Course
            {
                Title = "C# Basics",
                Description = "Learn C#",
                InstructorId = instructor.Id
            };

            // Act
            await _courseRepository.AddAsync(course);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _courseRepository.GetByIdAsync(course.Id);
            result.Should().NotBeNull();
            result!.Title.Should().Be("C# Basics");
        }

        [Fact]
        public async Task GetByTitleAsync_WithExistingTitle_ReturnsCourse()
        {
            // Arrange
            var instructor = new User
            {
                Name = "Dr. Smith",
                Email = "smith@example.com",
                PasswordHash = "hash",
                Role = "Instructor"
            };
            await _userRepository.AddAsync(instructor);
            await _context.SaveChangesAsync();

            var course = new Course
            {
                Title = "Advanced C#",
                Description = "Learn advanced C#",
                InstructorId = instructor.Id
            };
            await _courseRepository.AddAsync(course);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseRepository.GetByTitleAsync("Advanced C#");

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be("Advanced C#");
        }

        [Fact]
        public async Task GetPagedCoursesAsync_WithSearchTerm_FiltersCorrectly()
        {
            // Arrange
            var instructor = new User
            {
                Name = "Dr. Smith",
                Email = "smith@example.com",
                PasswordHash = "hash",
                Role = "Instructor"
            };
            await _userRepository.AddAsync(instructor);
            await _context.SaveChangesAsync();

            var courses = new List<Course>
            {
                new Course { Title = "C# Basics", Description = "Basics of C#", InstructorId = instructor.Id },
                new Course { Title = "JavaScript Basics", Description = "Basics of JavaScript", InstructorId = instructor.Id },
                new Course { Title = "C# Advanced", Description = "Advanced C#", InstructorId = instructor.Id }
            };

            foreach (var course in courses)
            {
                await _courseRepository.AddAsync(course);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseRepository.GetPagedCoursesAsync(1, 10, "C#");

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(c => c.Title.Should().Contain("C#"));
        }

        [Fact]
        public async Task GetPagedCoursesAsync_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange
            var instructor = new User
            {
                Name = "Dr. Smith",
                Email = "smith@example.com",
                PasswordHash = "hash",
                Role = "Instructor"
            };
            await _userRepository.AddAsync(instructor);
            await _context.SaveChangesAsync();

            for (int i = 1; i <= 25; i++)
            {
                var course = new Course
                {
                    Title = $"Course {i}",
                    Description = $"Description {i}",
                    InstructorId = instructor.Id
                };
                await _courseRepository.AddAsync(course);
            }
            await _context.SaveChangesAsync();

            // Act
            var page1 = await _courseRepository.GetPagedCoursesAsync(1, 10, null);
            var page2 = await _courseRepository.GetPagedCoursesAsync(2, 10, null);

            // Assert
            page1.Should().HaveCount(10);
            page2.Should().HaveCount(10);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
