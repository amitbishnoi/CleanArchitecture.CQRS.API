using Application.Common.Models;
using FluentAssertions;
using Xunit;

namespace Tests.xUnit.Common.Models
{
    public class PaginationParamsTests
    {
        [Fact]
        public void PaginationParams_WithDefaultValues_HasCorrectDefaults()
        {
            // Arrange & Act
            var pagination = new PaginationParams();

            // Assert
            pagination.PageNumber.Should().Be(1);
            pagination.PageSize.Should().Be(10);
            pagination.SearchTerm.Should().BeNull();
        }

        [Fact]
        public void PaginationParams_WithExceedingPageSize_CappsToMaxPageSize()
        {
            // Arrange & Act
            var pagination = new PaginationParams { PageSize = 100 };

            // Assert
            pagination.PageSize.Should().Be(50);
        }

        [Fact]
        public void PaginationParams_WithValidPageSize_AcceptsPageSize()
        {
            // Arrange & Act
            var pagination = new PaginationParams { PageSize = 25 };

            // Assert
            pagination.PageSize.Should().Be(25);
        }

        [Fact]
        public void PaginationParams_WithSearchTerm_SetSearchTerm()
        {
            // Arrange & Act
            var pagination = new PaginationParams { SearchTerm = "test-search" };

            // Assert
            pagination.SearchTerm.Should().Be("test-search");
        }

        [Fact]
        public void PaginationParams_WithPageSize1_AcceptsPageSize()
        {
            // Arrange & Act
            var pagination = new PaginationParams { PageSize = 1 };

            // Assert
            pagination.PageSize.Should().Be(1);
        }

        [Fact]
        public void PaginationParams_WithNegativePageSize_ShouldHandleGracefully()
        {
            // Arrange & Act
            var pagination = new PaginationParams { PageSize = -5 };

            // Assert - should keep the negative value or be handled by validator
            pagination.PageSize.Should().Be(-5);
        }
    }
}
