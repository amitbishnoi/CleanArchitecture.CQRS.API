using FluentValidation;

namespace Application.Features.Users.Queries.GetUser
{
    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIDQuery>
    {
        public GetUserByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("User ID must be greater than zero.");
        }
    }
}
