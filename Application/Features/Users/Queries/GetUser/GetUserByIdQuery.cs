using Application.Features.Users.Dtos;
using MediatR;

namespace Application.Features.Users.Queries.GetUser
{
    public class GetUserByIDQuery : IRequest<UserDto?>
    {
        public int Id { get; set; }

        public GetUserByIDQuery(int id)
        {
            Id = id;
        }
    }
}
