using Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace Tests.xUnit.Services
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;

        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void HashPassword_WithValidPassword_ReturnsHash()
        {
            // Arrange
            var password = "SecurePassword123!";

            // Act
            var hash = _passwordHasher.HashPassword(password);

            // Assert
            hash.Should().NotBeNull();
            hash.Should().NotBeEmpty();
            hash.Should().Contain(".");
        }

        [Fact]
        public void HashPassword_WithSamePassword_ProducesDifferentHashes()
        {
            // Arrange
            var password = "SecurePassword123!";

            // Act
            var hash1 = _passwordHasher.HashPassword(password);
            var hash2 = _passwordHasher.HashPassword(password);

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "SecurePassword123!";
            var hashedPassword = _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var correctPassword = "SecurePassword123!";
            var wrongPassword = "WrongPassword456!";
            var hashedPassword = _passwordHasher.HashPassword(correctPassword);

            // Act
            var result = _passwordHasher.VerifyPassword(wrongPassword, hashedPassword);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void VerifyPassword_WithMalformedHash_ReturnsFalse()
        {
            // Arrange
            var password = "SecurePassword123!";
            var malformedHash = "not-a-valid-hash";

            // Act
            var result = _passwordHasher.VerifyPassword(password, malformedHash);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var password = "SecurePassword123!";
            var hashedPassword = _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword("", hashedPassword);

            // Assert
            result.Should().BeFalse();
        }
    }
}
