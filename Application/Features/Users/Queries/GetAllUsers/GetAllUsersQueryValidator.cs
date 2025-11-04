using FluentValidation;

namespace Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
    {
        public GetAllUsersQueryValidator()
        {
            // No fields to validate — can leave empty
        }
    }
}
