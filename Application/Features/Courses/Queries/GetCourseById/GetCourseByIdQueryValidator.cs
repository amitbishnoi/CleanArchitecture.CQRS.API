using FluentValidation;

namespace Application.Features.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQueryValidator : AbstractValidator<GetCourseByIdQuery>
    {
        public GetCourseByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Course ID must be greater than zero.");
        }
    }
}
