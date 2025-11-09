using FluentValidation;

namespace Application.Features.Enrollment.Commands.CreateEnrollment
{
    public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
    {
        public CreateEnrollmentCommandValidator() 
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than zero.");
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Course ID must be greater than zero.");
        }
    }
}
