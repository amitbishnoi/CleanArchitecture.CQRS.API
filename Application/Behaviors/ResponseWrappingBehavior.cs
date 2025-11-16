using Application.Common.Responses;
using Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Application.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior that automatically wraps handler responses in ApiResponse<T>.
    /// This ensures all successful responses follow the standard API contract.
    /// 
    /// Usage: Register in DependencyInjection.cs
    /// services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResponseWrappingBehavior<,>));
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public class ResponseWrappingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<ResponseWrappingBehavior<TRequest, TResponse>> _logger;

        public ResponseWrappingBehavior(ILogger<ResponseWrappingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing request: {RequestType}", typeof(TRequest).Name);

            try
            {
                var response = await next();

                // If response is already ApiResponse, return as-is
                if (response is IApiResponse)
                {
                    _logger.LogInformation("Response already wrapped: {RequestType}", typeof(TRequest).Name);
                    return response;
                }

                // If response is Result<T>, wrap it
                if (IsResultType(response))
                {
                    _logger.LogInformation("Wrapping Result<T> response: {RequestType}", typeof(TRequest).Name);
                    return WrapResultResponse(response!);
                }

                // For other responses, wrap in ApiResponse
                _logger.LogInformation("Wrapping standard response: {RequestType}", typeof(TRequest).Name);
                return WrapStandardResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request: {RequestType}", typeof(TRequest).Name);
                throw;
            }
        }

        private static bool IsResultType(object? obj)
        {
            if (obj == null) return false;
            var objType = obj.GetType();
            return objType.IsGenericType &&
                   (objType.GetGenericTypeDefinition() == typeof(Result<>) ||
                    objType.BaseType?.GetGenericTypeDefinition() == typeof(Result<>));
        }

        private TResponse WrapResultResponse(object resultObj)
        {
            var resultType = resultObj.GetType();

            // Get the generic argument T from Result<T>
            var genericArg = resultType.GetGenericArguments()[0];

            // Create ApiResponse<T> type
            var apiResponseType = typeof(ApiResponse<>).MakeGenericType(genericArg);

            // Get Result<T>.IsSuccess property
            var isSuccessProperty = resultType.GetProperty("IsSuccess", BindingFlags.Public | BindingFlags.Instance);
            var isSuccess = (bool)isSuccessProperty!.GetValue(resultObj)!;

            if (isSuccess)
            {
                // Get Result<T>.Data property
                var dataProperty = resultType.GetProperty("Data", BindingFlags.Public | BindingFlags.Instance);
                var data = dataProperty!.GetValue(resultObj);

                // Call ApiResponse<T>.Ok(data)
                var okMethod = apiResponseType.GetMethod("Ok", BindingFlags.Public | BindingFlags.Static, null,
                    new[] { genericArg }, null);

                var wrappedResponse = okMethod!.Invoke(null, new[] { data });
                return (TResponse)(object)wrappedResponse!;
            }
            else
            {
                // Get error properties
                var errorMessageProperty = resultType.GetProperty("ErrorMessage", BindingFlags.Public | BindingFlags.Instance);
                var errorCodeProperty = resultType.GetProperty("ErrorCode", BindingFlags.Public | BindingFlags.Instance);

                var errorMessage = (string?)errorMessageProperty!.GetValue(resultObj) ?? "Operation failed";
                var errorCode = (int?)errorCodeProperty!.GetValue(resultObj);

                // Call ApiResponse<T>.Fail(message, statusCode, errorCode, error, traceId)
                var failMethod = apiResponseType.GetMethod("Fail", BindingFlags.Public | BindingFlags.Static);
                var wrappedResponse = failMethod!.Invoke(null, new object?[] { errorMessage, 400, errorCode, null, null });
                return (TResponse)(object)wrappedResponse!;
            }
        }

        private TResponse WrapStandardResponse(object? response)
        {
            // Check if TResponse is nullable type (e.g., CourseDto?, UserDto?)
            var responseType = typeof(TResponse);
            var underlyingType = Nullable.GetUnderlyingType(responseType) ?? responseType;

            // If response is null and TResponse is nullable, just return null as-is
            // This allows controllers to handle null responses (404, etc.)
            if (response == null)
            {
                // If TResponse is nullable (like CourseDto?), return null
                if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return default!;
                }

                // Otherwise wrap null in ApiResponse
                var apiResponseType = typeof(ApiResponse<>).MakeGenericType(typeof(object));
                var okMethod = apiResponseType.GetMethod("Ok", BindingFlags.Public | BindingFlags.Static, null,
                    new[] { typeof(string) }, null);

                var wrappedResponse = okMethod!.Invoke(null, new object[] { "Operation completed successfully" });
                return (TResponse)(object)wrappedResponse!;
            }

            // Response is not null, wrap it normally
            var actualResponseType = response.GetType();

            // If response is already a generic type (not primitive)
            if (actualResponseType.IsGenericType)
            {
                var apiResponseType = typeof(ApiResponse<>).MakeGenericType(actualResponseType);
                var okMethod = apiResponseType.GetMethod("Ok", BindingFlags.Public | BindingFlags.Static, null,
                    new[] { actualResponseType }, null);

                var wrappedResponse = okMethod!.Invoke(null, new[] { response });
                return (TResponse)(object)wrappedResponse!;
            }

            // For primitive types, wrap in ApiResponse<T> where T is the response type
            if (actualResponseType == typeof(int) || actualResponseType == typeof(string))
            {
                var apiResponseType = typeof(ApiResponse<>).MakeGenericType(actualResponseType);
                var okMethod = apiResponseType.GetMethod("Ok", BindingFlags.Public | BindingFlags.Static, null,
                    new[] { actualResponseType }, null);

                var wrappedResponse = okMethod!.Invoke(null, new[] { response });
                return (TResponse)(object)wrappedResponse!;
            }

            // Default: return as-is
            return (TResponse)response;
        }
    }

    /// <summary>
    /// Marker interface for already-wrapped responses
    /// </summary>
    public interface IApiResponse
    {
    }

    /// <summary>
    /// Extension methods for ApiResponse
    /// </summary>
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// Make ApiResponse implement IApiResponse marker interface
        /// </summary>
        public static T MarkAsWrapped<T>(this T response) where T : IApiResponse
        {
            return response;
        }
    }
}
