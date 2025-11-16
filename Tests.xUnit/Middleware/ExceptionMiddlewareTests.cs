using Application.Common.Enums;
using Application.Common.Exceptions;
using Application.Common.Responses;
using API.Middleware;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Moq;
using Microsoft.Extensions.Logging;
using Xunit;
using System;
using System.Collections.Generic;

namespace Tests.xUnit.Middleware
{
    public class ExceptionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WithApplicationException_ReturnsConsistentApiResponse()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/users";
            httpContext.Request.Method = "POST";
            httpContext.Response.Body = new MemoryStream();

            var logger = new Mock<ILogger<ExceptionMiddleware>>();
            var middleware = new ExceptionMiddleware(
                next: async (ctx) => throw new Application.Common.Exceptions.ApplicationException("Email already exists", ErrorCode.DuplicateEmail),
                logger.Object
            );

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            var response = JsonSerializer.Deserialize<ApiResponse<object>>(responseText, 
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            // Verify consistent response shape
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal(409, response.StatusCode); // Conflict
            Assert.Equal((int)ErrorCode.DuplicateEmail, response.ErrorCode);
            Assert.Equal("Email already exists", response.Message);
            Assert.NotNull(response.TraceId);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task InvokeAsync_WithKeyNotFoundException_ReturnsNotFoundResponse()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/users/999";
            httpContext.Request.Method = "GET";
            httpContext.Response.Body = new MemoryStream();

            var logger = new Mock<ILogger<ExceptionMiddleware>>();
            var middleware = new ExceptionMiddleware(
                next: async (ctx) => throw new KeyNotFoundException("User not found"),
                logger.Object
            );

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            var response = JsonSerializer.Deserialize<ApiResponse<object>>(responseText,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal(404, response.StatusCode);
            Assert.Equal((int)ErrorCode.UserNotFound, response.ErrorCode);
            Assert.Equal("User not found", response.Message);
        }

        [Fact]
        public async Task InvokeAsync_WithUnauthorizedAccessException_ReturnsUnauthorizedResponse()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var logger = new Mock<ILogger<ExceptionMiddleware>>();
            var middleware = new ExceptionMiddleware(
                next: async (ctx) => throw new UnauthorizedAccessException("Invalid credentials"),
                logger.Object
            );

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            var response = JsonSerializer.Deserialize<ApiResponse<object>>(responseText,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal(401, response.StatusCode);
            Assert.Equal((int)ErrorCode.Unauthorized, response.ErrorCode);
            Assert.Equal("Invalid credentials", response.Message);
        }

        [Fact]
        public async Task InvokeAsync_WithGenericException_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var logger = new Mock<ILogger<ExceptionMiddleware>>();
            var middleware = new ExceptionMiddleware(
                next: async (ctx) => throw new InvalidOperationException("Something went wrong"),
                logger.Object
            );

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            var response = JsonSerializer.Deserialize<ApiResponse<object>>(responseText,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal((int)ErrorCode.InternalServerError, response.ErrorCode);
            Assert.Equal("Something went wrong", response.Message);
        }

        [Fact]
        public async Task InvokeAsync_AllErrorResponses_HaveConsistentShape()
        {
            // Arrange
            var testCases = new[]
            {
                (ex: (Exception)new KeyNotFoundException("Not found"), expectedStatus: 404),
                (ex: new UnauthorizedAccessException("Unauthorized") as Exception, expectedStatus: 401),
                (ex: new InvalidOperationException("Error") as Exception, expectedStatus: 500),
            };

            foreach (var (exception, expectedStatus) in testCases)
            {
                // Act
                var httpContext = new DefaultHttpContext();
                httpContext.Response.Body = new MemoryStream();

                var logger = new Mock<ILogger<ExceptionMiddleware>>();
                var middleware = new ExceptionMiddleware(
                    next: async (ctx) => throw exception,
                    logger.Object
                );

                await middleware.InvokeAsync(httpContext);

                // Assert
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
                var response = JsonSerializer.Deserialize<ApiResponse<object>>(responseText,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                // All responses must have consistent shape
                Assert.NotNull(response);
                Assert.False(response.Success); // All errors have success=false
                Assert.Equal(expectedStatus, response.StatusCode);
                Assert.NotNull(response.ErrorCode); // All must have error code
                Assert.NotNull(response.Message); // All must have message
                Assert.NotNull(response.TraceId); // All must have trace ID
                Assert.Null(response.Data); // Error responses have no data
                Assert.NotNull(response.Error); // Error responses have error details
            }
        }

        [Fact]
        public async Task InvokeAsync_ResponseContentTypeIsJson()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var logger = new Mock<ILogger<ExceptionMiddleware>>();
            var middleware = new ExceptionMiddleware(
                next: async (ctx) => throw new Application.Common.Exceptions.ApplicationException("Test error", ErrorCode.ValidationError),
                logger.Object
            );

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            Assert.Equal("application/json", httpContext.Response.ContentType);
        }
    }
}
