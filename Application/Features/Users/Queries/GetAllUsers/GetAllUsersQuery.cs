using Application.Features.Users.Dtos;
using MediatR;

namespace Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<List<UserDto>>
    {
    }
}
