using FluentValidation;

namespace Application.Features.Courses.Commands.DeleteCourse
{
    /// <summary>
    /// Validator for DeleteCourseCommand to ensure course ID is valid
    /// </summary>
    public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
    {
        public DeleteCourseCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid course ID.");
        }
    }
}
