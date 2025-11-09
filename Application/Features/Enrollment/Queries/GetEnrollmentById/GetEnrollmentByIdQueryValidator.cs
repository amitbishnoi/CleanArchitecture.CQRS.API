using FluentValidation;

namespace Application.Features.Enrollment.Queries.GetEnrollmentById
{
    public class GetEnrollmentByIdQueryValidator : AbstractValidator<GetEnrollmentByIdQuery>
    {
        public GetEnrollmentByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Enrollment ID must be greater than zero.");
        }
    }
}