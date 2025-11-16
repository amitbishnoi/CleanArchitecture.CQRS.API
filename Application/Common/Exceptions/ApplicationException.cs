using Application.Common.Enums;

namespace Application.Common.Exceptions
{
    public class ApplicationException : Exception
    {
        public ErrorCode ErrorCode { get; set; }
        public string? Details { get; set; }

        public ApplicationException(string message, ErrorCode errorCode = ErrorCode.InternalServerError, string? details = null)
            : base(message)
        {
            ErrorCode = errorCode;
            Details = details;
        }
    }
}
