using Application.Features.Users.Dtos;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetAllUsersQuery, List<UserDto>>
    {
        public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetPagedUsersAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                request.Pagination.SearchTerm
            );

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            }).ToList();
        }
    }
}
