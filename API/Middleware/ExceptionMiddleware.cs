using System.Net;
using System.Text.Json;
using Application.Common.Enums;
using Application.Common.Responses;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound, ErrorCode.UserNotFound);
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.Unauthorized, ErrorCode.Unauthorized);
            }
            catch (Application.Common.Exceptions.ApplicationException ex)
            {
                var statusCode = ex.ErrorCode switch
                {
                    ErrorCode.ValidationError or ErrorCode.InvalidEmail or ErrorCode.InvalidPassword => HttpStatusCode.BadRequest,
                    ErrorCode.DuplicateEmail or ErrorCode.DuplicateEnrollment => HttpStatusCode.Conflict,
                    ErrorCode.UserNotFound or ErrorCode.CourseNotFound or ErrorCode.EnrollmentNotFound => HttpStatusCode.NotFound,
                    ErrorCode.Unauthorized or ErrorCode.InvalidCredentials or ErrorCode.TokenExpired => HttpStatusCode.Unauthorized,
                    ErrorCode.Forbidden => HttpStatusCode.Forbidden,
                    _ => HttpStatusCode.InternalServerError
                };

                await HandleApplicationExceptionAsync(context, ex, statusCode);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, ErrorCode.InternalServerError);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode, ErrorCode errorCode)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(
                ex,
                "An error occurred | TraceId: {TraceId} | Path: {Path} | Method: {Method} | StatusCode: {StatusCode} | ErrorCode: {ErrorCode}",
                traceId, context.Request.Path, context.Request.Method, (int)statusCode, (int)errorCode);

            var response = ApiResponse<object>.Fail(
                message: ex.Message,
                statusCode: (int)statusCode,
                errorCode: (int)errorCode,
                error: ErrorModel.FromException(ex, includeStackTrace: false),
                traceId: traceId
            );

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

        private Task HandleApplicationExceptionAsync(HttpContext context, Application.Common.Exceptions.ApplicationException ex, HttpStatusCode statusCode)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(
                ex,
                "Application error occurred | TraceId: {TraceId} | Path: {Path} | Method: {Method} | StatusCode: {StatusCode} | ErrorCode: {ErrorCode}",
                traceId, context.Request.Path, context.Request.Method, (int)statusCode, (int)ex.ErrorCode);

            var response = ApiResponse<object>.Fail(
                message: ex.Message,
                statusCode: (int)statusCode,
                errorCode: (int)ex.ErrorCode,
                error: ErrorModel.Create("ApplicationException", ex.Message),
                traceId: traceId
            );

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

        private static string GetTitleForStatusCode(HttpStatusCode statusCode) =>
            statusCode switch
            {
                HttpStatusCode.NotFound => "Resource not found",
                HttpStatusCode.BadRequest => "Invalid request",
                HttpStatusCode.Unauthorized => "Unauthorized access",
                HttpStatusCode.Forbidden => "Forbidden",
                HttpStatusCode.Conflict => "Resource conflict",
                HttpStatusCode.InternalServerError => "Internal server error",
                _ => "Unexpected error"
            };
    }
}
