using System.Text.Json.Serialization;

namespace Application.Common.Responses
{
    /// <summary>
    /// Standard API response wrapper for all endpoints.
    /// Provides consistent response structure for both success and error cases.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the response</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the request was successful (true) or failed (false)
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// HTTP status code (200, 201, 400, 404, 500, etc.)
        /// </summary>
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        /// Human-readable message describing the result
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Application-specific error code for categorized error handling
        /// Only populated on errors (e.g., 1001 for validation, 2001 for not found)
        /// </summary>
        [JsonPropertyName("errorCode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ErrorCode { get; set; }

        /// <summary>
        /// The actual response data (on success) or null (on error)
        /// </summary>
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        /// <summary>
        /// Error details (validation errors, stack trace in dev, etc.)
        /// </summary>
        [JsonPropertyName("details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ErrorModel? Error { get; set; }

        /// <summary>
        /// Pagination metadata if the response contains paginated data
        /// </summary>
        [JsonPropertyName("pagination")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PaginationMetadata? Pagination { get; set; }

        /// <summary>
        /// Trace ID for server-side error tracking and debugging
        /// </summary>
        [JsonPropertyName("traceId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TraceId { get; set; }

        /// <summary>
        /// Creates a successful response with data
        /// </summary>
        public static ApiResponse<T> Ok(T data, string message = "Request successful", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a successful response without data
        /// </summary>
        public static ApiResponse<T> Ok(string message = "Request successful", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = default
            };
        }

        /// <summary>
        /// Creates a successful response with pagination metadata
        /// </summary>
        public static ApiResponse<T> Ok(T data, PaginationMetadata pagination, string message = "Request successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = 200,
                Message = message,
                Data = data,
                Pagination = pagination
            };
        }

        /// <summary>
        /// Creates an error response
        /// </summary>
        public static ApiResponse<T> Fail(string message, int statusCode = 500, int? errorCode = null, ErrorModel? error = null, string? traceId = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                ErrorCode = errorCode,
                Error = error,
                TraceId = traceId
            };
        }

        /// <summary>
        /// Creates a validation error response
        /// </summary>
        public static ApiResponse<T> Validation(ErrorModel validationErrors, string message = "Validation failed", string? traceId = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = 400,
                Message = message,
                ErrorCode = 1001, // ValidationError code
                Error = validationErrors,
                TraceId = traceId
            };
        }

        /// <summary>
        /// Creates a not found error response
        /// </summary>
        public static ApiResponse<T> NotFound(string message, int errorCode = 2001, string? traceId = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = 404,
                Message = message,
                ErrorCode = errorCode,
                TraceId = traceId
            };
        }

        /// <summary>
        /// Creates a conflict error response
        /// </summary>
        public static ApiResponse<T> Conflict(string message, int errorCode = 4001, string? traceId = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = 409,
                Message = message,
                ErrorCode = errorCode,
                TraceId = traceId
            };
        }

        /// <summary>
        /// Creates an unauthorized error response
        /// </summary>
        public static ApiResponse<T> Unauthorized(string message, int errorCode = 3001, string? traceId = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = 401,
                Message = message,
                ErrorCode = errorCode,
                TraceId = traceId
            };
        }
    }

    /// <summary>
    /// Non-generic version for contexts where T is not known
    /// </summary>
    public class ApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("errorCode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ErrorCode { get; set; }

        [JsonPropertyName("details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ErrorModel? Error { get; set; }

        [JsonPropertyName("traceId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TraceId { get; set; }

        public static ApiResponse Fail(string message, int statusCode = 500, int? errorCode = null, string? traceId = null)
        {
            return new ApiResponse
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                ErrorCode = errorCode,
                TraceId = traceId
            };
        }
    }
}
