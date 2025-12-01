using FluentValidation;

namespace Application.Features.Enrollment.Commands.DeleteEnrollment
{
    /// <summary>
    /// Validator for DeleteEnrollmentCommand to ensure enrollment ID is valid
    /// </summary>
    public class DeleteEnrollmentCommandValidator : AbstractValidator<DeleteEnrollmentCommand>
    {
        public DeleteEnrollmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid enrollment ID.");
        }
    }
}
