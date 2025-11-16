using Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using System;
using Xunit;

namespace Tests.xUnit.Services
{
    public class CacheServiceTests
    {
        private readonly CacheService _cacheService;
        private readonly IMemoryCache _memoryCache;

        public CacheServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new CacheService(_memoryCache);
        }

        [Fact]
        public void Set_WithValidData_StoresInCache()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            var duration = TimeSpan.FromMinutes(5);

            // Act
            _cacheService.Set(key, value, duration);

            // Assert
            var retrieved = _cacheService.Get<string>(key);
            retrieved.Should().Be(value);
        }

        [Fact]
        public void Get_WithNonExistentKey_ReturnsDefault()
        {
            // Arrange
            var key = "non-existent-key";

            // Act
            var result = _cacheService.Get<string>(key);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Remove_WithValidKey_RemovesFromCache()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            _cacheService.Set(key, value, TimeSpan.FromMinutes(5));

            // Act
            _cacheService.Remove(key);
            var result = _cacheService.Get<string>(key);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void RemoveByPattern_WithMatchingPattern_RemovesMatching()
        {
            // Arrange
            var pattern = "users_";
            _cacheService.Set("users_1", "user1", TimeSpan.FromMinutes(5));
            _cacheService.Set("users_2", "user2", TimeSpan.FromMinutes(5));
            _cacheService.Set("courses_1", "course1", TimeSpan.FromMinutes(5));

            // Act
            _cacheService.RemoveByPattern(pattern);

            // Assert
            var user1 = _cacheService.Get<string>("users_1");
            var user2 = _cacheService.Get<string>("users_2");
            var course1 = _cacheService.Get<string>("courses_1");

            user1.Should().BeNull();
            user2.Should().BeNull();
            course1.Should().Be("course1");
        }

        [Fact]
        public void Set_WithObject_StoresAndRetrievesCorrectly()
        {
            // Arrange
            var key = "object-key";
            var obj = new { Id = 1, Name = "Test" };

            // Act
            _cacheService.Set(key, obj, TimeSpan.FromMinutes(5));
            var result = _cacheService.Get<object>(key);

            // Assert
            result.Should().NotBeNull();
        }
    }
}
