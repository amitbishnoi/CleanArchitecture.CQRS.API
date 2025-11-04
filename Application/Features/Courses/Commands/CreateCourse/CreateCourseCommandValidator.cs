using FluentValidation;

namespace Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

        }
    }
}
