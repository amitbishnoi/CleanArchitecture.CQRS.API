namespace Application.Common.Results
{
    /// <summary>
    /// Result<T> pattern for command handlers - encapsulates success or failure state.
    /// Provides a functional approach to error handling without throwing exceptions.
    /// </summary>
    /// <typeparam name="T">The type of data on success</typeparam>
    public class Result<T>
    {
        private Result(bool isSuccess, T? data, string? errorMessage, int? errorCode, Exception? exception)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Exception = exception;
        }

        /// <summary>
        /// Indicates whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates whether the operation failed
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// The successful result data (null if failed)
        /// </summary>
        public T? Data { get; }

        /// <summary>
        /// Error message (null if successful)
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// Application error code for categorized handling (null if successful)
        /// </summary>
        public int? ErrorCode { get; }

        /// <summary>
        /// Exception if any (null if successful or error handled gracefully)
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Creates a successful result with data
        /// </summary>
        public static Result<T> Success(T data)
        {
            return new Result<T>(isSuccess: true, data: data, errorMessage: null, errorCode: null, exception: null);
        }

        /// <summary>
        /// Creates a failed result with error details
        /// </summary>
        public static Result<T> Failure(string errorMessage, int? errorCode = null, Exception? exception = null)
        {
            return new Result<T>(isSuccess: false, data: default, errorMessage: errorMessage, errorCode: errorCode, exception: exception);
        }

        /// <summary>
        /// Creates a failed result from an exception
        /// </summary>
        public static Result<T> FailureFromException(Exception exception, int? errorCode = null)
        {
            return new Result<T>(isSuccess: false, data: default, errorMessage: exception.Message, errorCode: errorCode, exception: exception);
        }

        /// <summary>
        /// Projects the result to another type
        /// </summary>
        public Result<TNew> Map<TNew>(Func<T, TNew> transform)
        {
            if (IsFailure)
            {
                return Result<TNew>.Failure(ErrorMessage ?? "Operation failed", ErrorCode, Exception);
            }

            try
            {
                var mappedData = transform(Data!);
                return Result<TNew>.Success(mappedData);
            }
            catch (Exception ex)
            {
                return Result<TNew>.FailureFromException(ex, ErrorCode);
            }
        }

        /// <summary>
        /// Chains multiple operations, short-circuiting on first failure
        /// </summary>
        public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> transform)
        {
            if (IsFailure)
            {
                return Result<TNew>.Failure(ErrorMessage ?? "Operation failed", ErrorCode, Exception);
            }

            try
            {
                return transform(Data!);
            }
            catch (Exception ex)
            {
                return Result<TNew>.FailureFromException(ex, ErrorCode);
            }
        }

        /// <summary>
        /// Executes a function if successful, returns self for chaining
        /// </summary>
        public Result<T> Tap(Action<T> action)
        {
            if (IsSuccess)
            {
                action(Data!);
            }
            return this;
        }

        /// <summary>
        /// Executes a function if failed, returns self for chaining
        /// </summary>
        public Result<T> TapError(Action<string, int?> action)
        {
            if (IsFailure)
            {
                action(ErrorMessage!, ErrorCode);
            }
            return this;
        }

        /// <summary>
        /// Gets the value or throws if failed
        /// </summary>
        public T Unwrap()
        {
            if (IsSuccess)
            {
                return Data!;
            }

            if (Exception != null)
            {
                throw Exception;
            }

            throw new InvalidOperationException(ErrorMessage);
        }

        /// <summary>
        /// Gets the value or returns default if failed
        /// </summary>
        public T? UnwrapOrDefault(T? defaultValue = default)
        {
            return IsSuccess ? Data : defaultValue;
        }

        /// <summary>
        /// Executes and returns the result of onSuccess if successful, else onFailure
        /// </summary>
        public TResult Match<TResult>(
            Func<T, TResult> onSuccess,
            Func<string, int?, TResult> onFailure)
        {
            return IsSuccess
                ? onSuccess(Data!)
                : onFailure(ErrorMessage!, ErrorCode);
        }

        /// <summary>
        /// Executes either onSuccess or onFailure based on result state
        /// </summary>
        public void Match(
            Action<T> onSuccess,
            Action<string, int?> onFailure)
        {
            if (IsSuccess)
            {
                onSuccess(Data!);
            }
            else
            {
                onFailure(ErrorMessage!, ErrorCode);
            }
        }
    }

    /// <summary>
    /// Non-generic Result for void operations
    /// </summary>
    public class Result
    {
        protected Result(bool isSuccess, string? errorMessage, int? errorCode, Exception? exception)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Exception = exception;
        }

        /// <summary>
        /// Indicates whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates whether the operation failed
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Error message (null if successful)
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// Application error code (null if successful)
        /// </summary>
        public int? ErrorCode { get; }

        /// <summary>
        /// Exception if any
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Creates a successful result
        /// </summary>
        public static Result Success()
        {
            return new Result(isSuccess: true, errorMessage: null, errorCode: null, exception: null);
        }

        /// <summary>
        /// Creates a failed result
        /// </summary>
        public static Result Failure(string errorMessage, int? errorCode = null, Exception? exception = null)
        {
            return new Result(isSuccess: false, errorMessage: errorMessage, errorCode: errorCode, exception: exception);
        }

        /// <summary>
        /// Creates a failed result from an exception
        /// </summary>
        public static Result FailureFromException(Exception exception, int? errorCode = null)
        {
            return new Result(isSuccess: false, errorMessage: exception.Message, errorCode: errorCode, exception: exception);
        }

        /// <summary>
        /// Converts to Result<T>
        /// </summary>
        public Result<T> ToGeneric<T>(T? data = default)
        {
            return IsSuccess
                ? Result<T>.Success(data!)
                : Result<T>.Failure(ErrorMessage!, ErrorCode, Exception);
        }

        /// <summary>
        /// Executes if successful, returns self for chaining
        /// </summary>
        public Result Tap(Action action)
        {
            if (IsSuccess)
            {
                action();
            }
            return this;
        }

        /// <summary>
        /// Executes if failed, returns self for chaining
        /// </summary>
        public Result TapError(Action<string, int?> action)
        {
            if (IsFailure)
            {
                action(ErrorMessage!, ErrorCode);
            }
            return this;
        }

        /// <summary>
        /// Executes either onSuccess or onFailure
        /// </summary>
        public void Match(
            Action onSuccess,
            Action<string, int?> onFailure)
        {
            if (IsSuccess)
            {
                onSuccess();
            }
            else
            {
                onFailure(ErrorMessage!, ErrorCode);
            }
        }
    }
}
