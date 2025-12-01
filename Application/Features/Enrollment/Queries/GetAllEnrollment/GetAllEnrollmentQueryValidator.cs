using FluentValidation;

namespace Application.Features.Enrollment.Queries.GetAllEnrollment
{
    /// <summary>
    /// Validator for GetAllEnrollmentQuery to ensure pagination parameters are valid
    /// </summary>
    public class GetAllEnrollmentQueryValidator : AbstractValidator<GetAllEnrollmentQuery>
    {
        public GetAllEnrollmentQueryValidator()
        {
            RuleFor(x => x.Pagination.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than zero.");

            RuleFor(x => x.Pagination.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than zero.")
                .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

            RuleFor(x => x.Pagination.SearchTerm)
                .MaximumLength(100).WithMessage("Search term must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Pagination.SearchTerm));
        }
    }
}
