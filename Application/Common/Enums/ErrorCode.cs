namespace Application.Common.Enums
{
    public enum ErrorCode
    {
        // Validation Errors (1000-1999)
        ValidationError = 1001,
        InvalidEmail = 1002,
        InvalidPassword = 1003,
        DuplicateEmail = 1004,
        
        // Resource Not Found (2000-2999)
        UserNotFound = 2001,
        CourseNotFound = 2002,
        EnrollmentNotFound = 2003,
        InstructorNotFound = 2004,
        
        // Authentication & Authorization (3000-3999)
        Unauthorized = 3001,
        InvalidCredentials = 3002,
        TokenExpired = 3003,
        Forbidden = 3004,
        
        // Conflict/Duplicate (4000-4999)
        DuplicateEnrollment = 4001,
        CourseAlreadyExists = 4002,
        
        // Server Errors (5000-5999)
        InternalServerError = 5000,
        DatabaseError = 5001,
        EmailSendError = 5002,
        TransactionError = 5003
    }
}
