using System.Text.Json.Serialization;

namespace Application.Common.Responses
{
    /// <summary>
    /// Represents error details in API responses.
    /// Can contain validation errors, exception details, or custom error information.
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Error title/category (e.g., "Validation Error", "Database Error")
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed error message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dictionary of field-level validation errors
        /// Key: field name, Value: error message
        /// Example: { "email": "Email is required", "password": "Password must be at least 8 characters" }
        /// </summary>
        [JsonPropertyName("fieldErrors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? FieldErrors { get; set; }

        /// <summary>
        /// Inner exception message (in development only)
        /// </summary>
        [JsonPropertyName("innerException")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? InnerException { get; set; }

        /// <summary>
        /// Stack trace (in development only)
        /// </summary>
        [JsonPropertyName("stackTrace")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? StackTrace { get; set; }

        /// <summary>
        /// Creates an error model from validation failures
        /// </summary>
        public static ErrorModel FromValidationFailures(List<(string Field, string Message)> failures)
        {
            var fieldErrors = failures
                .GroupBy(x => x.Field)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join("; ", g.Select(x => x.Message))
                );

            return new ErrorModel
            {
                Title = "Validation Error",
                Message = $"Validation failed with {failures.Count} error(s)",
                FieldErrors = fieldErrors
            };
        }

        /// <summary>
        /// Creates an error model from an exception
        /// </summary>
        public static ErrorModel FromException(Exception ex, bool includeStackTrace = false)
        {
            return new ErrorModel
            {
                Title = ex.GetType().Name,
                Message = ex.Message,
                InnerException = ex.InnerException?.Message,
                StackTrace = includeStackTrace ? ex.StackTrace : null
            };
        }

        /// <summary>
        /// Creates a simple error model
        /// </summary>
        public static ErrorModel Create(string title, string message, Dictionary<string, string>? fieldErrors = null)
        {
            return new ErrorModel
            {
                Title = title,
                Message = message,
                FieldErrors = fieldErrors
            };
        }
    }
}
