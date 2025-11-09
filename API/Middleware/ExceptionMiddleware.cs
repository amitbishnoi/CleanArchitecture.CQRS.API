using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate _next, ILogger<ExceptionMiddleware> _logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.Unauthorized);
            }
            catch (ApplicationException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode code)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(
                ex,
                "An error occurred | TraceId: {TraceId} | Path: {Path} | Method: {Method} | StatusCode: {StatusCode}",
                traceId, context.Request.Path, context.Request.Method, (int)code);
            var response = new
            {
                traceId,
                status = (int)code,
                title = GetTitleForStatusCode(code),
                message = ex.Message,
                details = ex.InnerException?.Message
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
        private static string GetTitleForStatusCode(HttpStatusCode statusCode) =>
            statusCode switch
            {
                HttpStatusCode.NotFound => "Resource not found",
                HttpStatusCode.BadRequest => "Invalid request",
                HttpStatusCode.Unauthorized => "Unauthorized access",
                HttpStatusCode.InternalServerError => "Internal server error",
                _ => "Unexpected error"
            };
    }
}
