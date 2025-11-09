using FluentValidation;

namespace Application.Features.Enrollment.Commands.UpdateEnrollment
{
    public class UpdateEnrollmentCommandValidator : AbstractValidator<UpdateEnrollmentCommand>
    {
        public UpdateEnrollmentCommandValidator() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Enrollment ID must be greater than zero.");
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than zero.");
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Course ID must be greater than zero.");
        }
    }
}
